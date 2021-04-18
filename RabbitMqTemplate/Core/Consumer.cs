using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqTemplate.Core
{
    public class Consumer
    {
        public static void Run()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://user:bitnami@192.168.235.129:5672")
            };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                "demo-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine(message);
            };

            channel.BasicConsume(
                "demo-queue",
                true,
                consumer);

            Console.ReadLine();
        }
    }
}