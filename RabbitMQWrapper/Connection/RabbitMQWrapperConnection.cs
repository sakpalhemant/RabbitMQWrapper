using RabbitMQWrapper.Channel;
using RabbitMQWrapper.Consumer;
using RabbitMQWrapper.Exceptions;
using RabbitMQWrapper.Serializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace RabbitMQWrapper.Connection
{
    public class RabbitMQWrapperConnection : IRabbitMQWrapperConnection
    {
        private string applicationKey;
        private AbstractChannelManager channelManager;

        public static AbstractSerializerAdapter DefaultAdapter { get; set; }

        public RabbitMQWrapperConnection(IModel channel, string applicationKey, string hostname)
        {
            try
            {
                this.applicationKey = applicationKey;

                this.channelManager = new ChannelManager(channel, applicationKey);
            }
            catch (Exception inner)
            {
                Exception custom = new OpenConnectionException(hostname, inner);
                throw custom;
            }

            var jsonSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver(),
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            DefaultAdapter = new NewtonSoftSerializerAdapter(jsonSettings);
        }

        public void Publish<T>(IBasicProperties basicProperties, string exchange, string routingKey, int eventId, string eventName, T content, string contractVersion = "1")
        {
            var message = new RabbitMQWrapperMessage<T>()
            {
                Event = eventName,
                AppKey = this.applicationKey,
                Uri = null,
                Exchange = exchange,
                RoutingKey = routingKey,
                Content = content,
                ContractVersion = contractVersion,
                EventID = eventId
            };
            this.channelManager.Publish(basicProperties, exchange, routingKey, message);
        }

        public void Publish<T>(IBasicProperties basicProperties, string exchange, string routingKey, string eventName, T content, string contractVersion = "1")
        {
            this.Publish(basicProperties, exchange, routingKey, Int32.MinValue, eventName, content, contractVersion);
        }

        public Task PublishAsync<T>(IBasicProperties basicProperties, string exchange, string routingKey, int eventId, string eventName, T content, string contractVersion = "1")
        {
            return Task.Factory.StartNew(() => this.Publish(basicProperties, exchange, routingKey, eventId, eventName, content, contractVersion));
        }

        public void Publish<T>(IBasicProperties basicProperties, string exchange, string routingKey, int eventId, T content, string contractVersion = "1")
        {
            this.Publish(basicProperties, exchange, routingKey, eventId, "", content, contractVersion);
        }

        public Task PublishAsync<T>(IBasicProperties basicProperties, string exchange, string routingKey, int eventId, T content, string contractVersion = "1")
        {
            return this.PublishAsync(basicProperties, exchange, routingKey, eventId, "", content, contractVersion);
        }

        public Task PublishAsync<T>(IBasicProperties basicProperties, string exchange, string routingKey, string eventName, T content, string contractVersion = "1")
        {
            return this.PublishAsync(basicProperties, exchange, routingKey, Int32.MinValue, eventName, content, contractVersion);
        }

        public void Consume<T>(IBasicProperties basicProperties, AbstractConsumer<T> consumer, string queue, string errorQueue, string exchange, string routingKey)
        {
            this.channelManager.RegisterChannel(basicProperties, consumer, queue, errorQueue, exchange, routingKey);
        }

        public void Consume<T>(IBasicProperties basicProperties, AbstractConsumerFactory<T> factory, string queue, string errorQueue, string exchange, string routingKey)
        {
            this.channelManager.RegisterChannel(basicProperties, factory, queue, errorQueue, exchange, routingKey);
        }

        public void Close()
        {
            this.channelManager.Dispose();
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}