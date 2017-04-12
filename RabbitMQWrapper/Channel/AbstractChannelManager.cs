using RabbitMQWrapper.Connection;
using RabbitMQWrapper.Consumer;
using RabbitMQ.Client;
using System;

namespace RabbitMQWrapper.Channel
{
    internal abstract class AbstractChannelManager : IDisposable
    {
        internal abstract void Publish<T>(IBasicProperties basicProperties, string exchange, string routingKey, RabbitMQWrapperMessage<T> message);

        internal abstract void RegisterChannel<T>(IBasicProperties basicProperties, AbstractConsumer<T> consumer, string queue, string errorQueue, string exchange, string routingKey);

        internal abstract void RegisterChannel<T>(IBasicProperties basicProperties, AbstractConsumerFactory<T> factory, string queue, string errorQueue, string exchange, string routingKey);

        public abstract void Dispose();
    }
}