using System;

namespace RabbitMQWrapper.Exceptions
{
    public class UnexpectedException : Exception
    {
        public UnexpectedException(Exception e) : base(innerException: e, message: "An unknown error occurred")
        {
        }
    }
}