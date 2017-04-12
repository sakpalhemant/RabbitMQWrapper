namespace RabbitMQWrapper.Consumer
{
    public abstract class AbstractConsumer<T>
    {
        public virtual void Consume(string eventName, T content, string contractVersion)
        {
        }

        public virtual void Consume(int eventId, T content, string contractVersion)
        {
        }
    }
}