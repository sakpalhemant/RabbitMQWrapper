using System;

namespace RabbitMQWrapper.Connection
{
    internal class RabbitMQWrapperMessage<T>
    {
        public Guid UUID { get; private set; }
        public String ContractVersion { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Event { get; set; }
        public string AppKey { get; set; }
        public Uri Uri { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public T Content { get; set; }
        public int EventID { get; set; }

        public RabbitMQWrapperMessage()
        {
            this.UUID = Guid.NewGuid();
            this.CreatedAt = DateTime.UtcNow;
            this.ContractVersion = "1";
            this.EventID = 0;
        }
    }
}