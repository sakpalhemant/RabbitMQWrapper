using RabbitMQWrapper.Connection;
using RabbitMQ.Client;
using Topshelf;

namespace Demo_Consumer_Complex
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<IRunnableService>(s =>
                {
                    s.ConstructUsing(name => new RabbitMQWrapperService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
            });
        }
    }
}