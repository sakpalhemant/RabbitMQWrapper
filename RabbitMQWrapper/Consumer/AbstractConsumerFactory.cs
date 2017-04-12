using System;

namespace RabbitMQWrapper.Consumer
{
    public abstract class AbstractConsumerFactory<T>
    {
        public virtual AbstractConsumer<T> ResolveEvent(string eventName, string contractVersion)
        {
            throw new NotImplementedException("Method not implemented!");
        }

        public virtual AbstractConsumer<T> ResolveEvent(int eventId, string contractVersion)
        {
            throw new NotImplementedException("Method not implemented!");
        }
    }
}