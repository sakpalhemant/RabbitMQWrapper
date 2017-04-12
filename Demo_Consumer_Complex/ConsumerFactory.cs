using RabbitMQWrapper.Consumer;

namespace Demo_Consumer_Complex
{
    public class ConsumerFactory : AbstractConsumerFactory<Message>
    {
        public override AbstractConsumer<Message> ResolveEvent(string eventName, string contractVersion)
        {
            if (eventName == "MessageTypeFoo")
            {
                return new FooConsumer();
            }
            else
            {
                return new BarConsumer();
            }
        }
    }
}