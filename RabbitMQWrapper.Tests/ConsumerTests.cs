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
    public class ConsumerTests
    {
        [Test]
        public void ShouldConsume()
        {
            var queue = ConfigurationManager.AppSettings.Get("RabbitMQ_Queue");
            var errorQueue = ConfigurationManager.AppSettings.Get("RabbitMQ_ErrorQueue");
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
                //channel.ExchangeDeclare(exchange: exchange, type: "topic");
                //var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queue,
                                  exchange: exchange,
                                  routingKey: routing);

                using (var rabbitMQWrapperConnection = new RabbitMQWrapperConnection(channel, appkey, uri))
                {
                    IBasicProperties basicProperties = channel.CreateBasicProperties();
                    basicProperties.DeliveryMode = 2;
                    basicProperties.Headers = new Dictionary<string, object>();
                    basicProperties.Headers.Add("header1", "test header");
                    //TODO: Publish

                    rabbitMQWrapperConnection.Consume(basicProperties, new Consumer(), queue, "", exchange, routing);

                    //TODO: check how many times called consumer
                }
            };
        }

        [Test]
        public void ShouldNotConsume()
        {
        }
    }
}