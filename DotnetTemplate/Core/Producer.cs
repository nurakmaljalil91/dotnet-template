using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace DotnetTemplate.Core
{
    public class Producer
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

            var message = new
            {
                Name = "Producer",
                Message = "Hello!"
            };

            var body = Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(message));

            channel.BasicPublish(
                "",
                "demo-queue",
                null,
                body);



        }
    }
}