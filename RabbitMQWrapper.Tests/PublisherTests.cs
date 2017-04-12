using RabbitMQWrapper.Connection;
using NUnit.Framework;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQWrapper.Tests
{
    [TestFixture]
    public class PublisherTests
    {
        [Test]
        public void ShouldSend()
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
                    IBasicProperties props = channel.CreateBasicProperties();
                    props.DeliveryMode = 2;
                    props.Headers = new Dictionary<string, object>();
                    props.Headers.Add("header1", "test header");

                    rabbitMQWrapperConnection.Publish<Message>(props, exchange, routing, "testevent", new Message() { PeristencyStatus = "Persistent", PlayerName = "PlayerName" });
                };
            }
        }

        [Test]
        public void ShouldNotSend()
        {
        }
    }
}