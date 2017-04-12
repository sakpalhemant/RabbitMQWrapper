using System;

namespace RabbitMQWrapper.Exceptions
{
    public class OpenConnectionException : Exception
    {
        public string HostName { get; set; }

        public OpenConnectionException(string hostName, Exception innerException) : base("Could not open a connection", innerException)
        {
            this.HostName = hostName;
        }
    }
}