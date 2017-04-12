using RabbitMQWrapper.Consumer;
using System;

namespace Demo_Consumer
{
    public class Consumer : AbstractConsumer<Message>
    {
        public override void Consume(int eventId, Message content, string contractVersion)
        {
            Console.Write("Message received! Player:" + content.PlayerName);
        }

        public override void Consume(string eventName, Message content, string contractVersion)
        {
            Console.Write("Message received! Player:" + content.PlayerName);
        }
    }
}