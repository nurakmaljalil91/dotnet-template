using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqTemplate.Core
{
    public class GlobalIndexConsumer
    {
        public static void Consume(IModel channel)
        {
            channel.ExchangeDeclare(
                "global_index",
                ExchangeType.Direct);

            channel.QueueDeclare(
                "global_index",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(
                "global_index",
                "global_index",
                "global_index");

            channel.BasicQos(
                0,
                10,
                false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine(message);
            };

            channel.BasicConsume(
                "global_index",
                true,
                consumer);

            Console.WriteLine("Consumer started");

            Console.ReadLine();
        }
    }
}