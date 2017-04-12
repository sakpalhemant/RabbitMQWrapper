using System;

namespace RabbitMQWrapper.Exceptions
{
    public class ConnectionClosedException : Exception
    {
        public ConnectionClosedException(Exception innerException) : base("Connection has reached the timeout", innerException)
        {
        }
    }
}