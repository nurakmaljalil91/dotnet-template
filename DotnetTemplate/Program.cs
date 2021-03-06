using System;
using DotnetTemplate.Core;
using RabbitMQ.Client;

namespace DotnetTemplate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://user:bitnami@192.168.235.128:5672")
            };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            DirectExchangePublisher.Publish(channel);

            //var simpleDate = new {Timestamp = "2021-04-25"};
        }
    }
}
