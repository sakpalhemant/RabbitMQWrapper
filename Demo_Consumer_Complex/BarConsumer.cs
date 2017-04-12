using RabbitMQWrapper.Consumer;
using System;

namespace Demo_Consumer_Complex
{
    public class BarConsumer : AbstractConsumer<Message>
    {
        public override void Consume(string eventName, Message content, String contractVersion = "1")
        {
            Console.Write("Message received. Player Name: ! " + content.PlayerName);
        }

        public override void Consume(int eventId, Message content, string contractVersion)
        {
            Console.Write("Message received. Player Name: ! " + content.PlayerName);
        }
    }
}