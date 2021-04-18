using System;
using RabbitMQ.Client;
using RabbitMqTemplate.Core;

namespace RabbitMqTemplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://user:bitnami@192.168.235.129:5672")
            };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            //DirectExchangePublisher.Publish(channel);
            DirectExchangeConsumer.Consume(channel);
        }
    }
}
