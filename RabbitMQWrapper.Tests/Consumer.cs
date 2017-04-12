using RabbitMQWrapper.Consumer;
using System;

namespace RabbitMQWrapper.Tests
{
    public class Consumer : AbstractConsumer<Message>
    {
        public override void Consume(int eventId, Message content, string contractVersion)
        {
            Console.Write("Message received! Player:" + content.PlayerName);
        }
    }
}