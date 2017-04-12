using System;

namespace RabbitMQWrapper.Exceptions
{
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException(Exception innerException) : base("Access denied", innerException)
        { }
    }
}