using RabbitMQWrapper.Consumer;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace RabbitMQWrapper.Connection
{
    public interface IRabbitMQWrapperConnection : IDisposable
    {
        void Consume<T>(IBasicProperties basicProperties, AbstractConsumer<T> consumer, string queue, string errorQueue, string exchange, string routingKey);

        void Consume<T>(IBasicProperties basicProperties, AbstractConsumerFactory<T> factory, string queue, string errorQueue, string exchange, string routingKey);

        void Publish<T>(IBasicProperties basicProperties, string exchange, string routingKey, string eventName, T content, string contractVersion = "1");

        void Publish<T>(IBasicProperties basicProperties, string exchange, string routingKey, int eventId, T content, string contractVersion = "1");

        void Publish<T>(IBasicProperties basicProperties, string exchange, string routingKey, int eventId, string eventName, T content, string contractVersion = "1");

        Task PublishAsync<T>(IBasicProperties basicProperties, string exchange, string routingKey, string eventName, T content, string contractVersion = "1");

        Task PublishAsync<T>(IBasicProperties basicProperties, string exchange, string routingKey, int eventId, T content, string contractVersion = "1");

        Task PublishAsync<T>(IBasicProperties basicProperties, string exchange, string routingKey, int eventId, string eventName, T content, string contractVersion = "1");

        void Close();
    }
}