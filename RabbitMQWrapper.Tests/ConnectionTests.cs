using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQWrapper.Connection;
using System;
using System.Configuration;

namespace RabbitMQWrapper.Tests
{
    [TestFixture]
    public class ConnectionTests
    {
        [Test]
        public void ShouldConnect()
        {
            var queue = ConfigurationManager.AppSettings.Get("RabbitMQ_Queue");
            var exchange = ConfigurationManager.AppSettings.Get("RabbitMQ_Exchange");
            var routing = ConfigurationManager.AppSettings.Get("RabbitMQ_RoutingKey");
            var uri = ConfigurationManager.AppSettings.Get("RabbitMQ_Address");

            var connectionFactory = new ConnectionFactory()
            {
                UserName = ConfigurationManager.AppSettings.Get("RabbitMQ_Username"),
                Password = ConfigurationManager.AppSettings.Get("RabbitMQ_Password")
            };

            using (var connection = connectionFactory.CreateConnection())
            {
                var appkey = ConfigurationManager.AppSettings.Get("AppKey");

                var channel = connection.CreateModel();
                using (var rabbitMQWrapperConnection = new RabbitMQWrapperConnection(channel, appkey, uri))
                {
                }
            }
        }

        [Test]
        public void ShouldNotConnect()
        {
        }
    }
}