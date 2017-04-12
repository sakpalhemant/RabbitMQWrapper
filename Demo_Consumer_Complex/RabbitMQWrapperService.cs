using RabbitMQWrapper.Connection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Demo_Consumer_Complex
{
    public class RabbitMQWrapperService : IRunnableService
    {
        public void Start()
        {
            StartRabbitMQWrapperConsumer();
        }

        public void StartRabbitMQWrapperConsumer()
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
                using (var rabbitMQWrapperConnection = new RabbitMQWrapperConnection(channel, appkey, uri))
                {
                    IBasicProperties basicProperties = channel.CreateBasicProperties();
                    basicProperties.DeliveryMode = 2;
                    basicProperties.Headers = new Dictionary<string, object>();
                    basicProperties.Headers.Add("header1", "test header");

                    rabbitMQWrapperConnection.Consume(basicProperties, new ConsumerFactory(), queue, "", exchange, routing);
                }
            };
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}