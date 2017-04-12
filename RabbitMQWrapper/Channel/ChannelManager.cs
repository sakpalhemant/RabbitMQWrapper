using Common.Logging;
using RabbitMQWrapper.Connection;
using RabbitMQWrapper.Consumer;
using RabbitMQWrapper.Exceptions;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Threading;

namespace RabbitMQWrapper.Channel
{
    internal class ChannelManager : AbstractChannelManager
    {
        private static ILog Logger = LogManager.GetLogger<ChannelManager>();
        public IModel channel;
        public string appKey;

        internal ChannelManager(IModel channel, string appKey)
        {
            this.channel = channel;
            this.appKey = appKey;
        }

        internal override void Publish<T>(IBasicProperties basicProperties, string exchange, string routingKey, RabbitMQWrapperMessage<T> message)
        {
            this.PublishMessage(basicProperties, exchange, routingKey, message);
            Logger.Info("Message has been published. Message UUID: " + message.UUID);
        }

        internal override void RegisterChannel<T>(IBasicProperties basicProperties, AbstractConsumer<T> consumer, string queue, string errorQueue, string exchange, string routingKey)
        {
            Logger.Info("Consumer has been registering. Consumer: " + consumer + ", queue: " + queue + ", errorRoute: " + errorQueue);
            ConsumerResolver<T> resolver = (int eventId, string eventName, string contractVersion) =>
            {
                return consumer;
            };

            this.ChannelRegistration(basicProperties, queue, errorQueue, exchange, routingKey, resolver);
        }

        internal override void RegisterChannel<T>(IBasicProperties basicProperties, AbstractConsumerFactory<T> factory, string queue, string errorQueue, string exchange, string routingKey)
        {
            Logger.Info("Registering... Consumer: " + factory + ", route: " + queue + ", errorRoute: " + errorQueue);

            ConsumerResolver<T> resolver = (int eventId, string eventName, string contractVersion) =>
            {
                if (eventId == Int32.MinValue)
                {
                    return factory.ResolveEvent(eventName, contractVersion);
                }
                return factory.ResolveEvent(eventId, contractVersion);
            };

            this.ChannelRegistration(basicProperties, queue, errorQueue, exchange, routingKey, resolver);
        }

        private void PublishMessage<T>(IBasicProperties basicProperties, string exchange, string routingKey, RabbitMQWrapperMessage<T> message)
        {
            var serializer = RabbitMQWrapperConnection.DefaultAdapter;
            var body = serializer.SerializeBytes(message);

            try
            {
                channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: basicProperties, body: body);
            }
            catch (Exception e)
            {
                var custom = new UnexpectedException(e);
                Logger.Fatal("Unexpected error occured while publishing the message", custom);
                throw custom;
            }
        }

        private delegate AbstractConsumer<T> ConsumerResolver<T>(int eventId, string eventName, string contractVersion);

        private void ChannelRegistration<T>(IBasicProperties basicProperties, string queue, string errorQueue, string exchange, string routingKey, ConsumerResolver<T> resolver)
        {
            var rabbitConsumer = new EventingBasicConsumer(channel);
            rabbitConsumer.Received += (model, ea) =>
            {
                var serializer = RabbitMQWrapperConnection.DefaultAdapter;
                try
                {
                    var message = serializer.DeserializeBytes<RabbitMQWrapperMessage<T>>(ea.Body);

                    AbstractConsumer<T> consumer = null;
                    try
                    {
                        consumer = resolver(message.EventID, message.Event, message.ContractVersion);

                        if (message.Event == "")
                        {
                            consumer.Consume(message.EventID, message.Content, message.ContractVersion);
                        }
                        consumer.Consume(message.Event, message.Content, message.ContractVersion);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Consumer: " + consumer + " has unexpected error", e);
                        channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                        return;
                    }

                    try
                    {
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Connection lost", e);
                        throw e;
                    }
                }
                catch (JsonException e)
                {
                    var customError = new UnexpectedException(e);
                    Logger.Error("Could not deserialize the object: " + ea.Body, customError);
                    try
                    {
                        Logger.Warn("Creating error queue...");
                        this.EnsureRoute(errorQueue, exchange, routingKey);
                        this.channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: basicProperties, body: ea.Body);
                        channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                    catch (Exception ex)
                    {
                        var custom = new UnexpectedException(ex);
                        Logger.Fatal("Unexpected error occured while publishing items to error queue", custom);
                        throw custom;
                    }
                }
                catch (Exception e)
                {
                    var custom = new UnexpectedException(e);
                    Logger.Fatal("Unexpected error occured while consumer received a message", custom);
                    throw custom;
                }
            };

            try
            {
                channel.BasicConsume(queue: queue, noAck: false, consumer: rabbitConsumer, consumerTag: appKey);
            }
            catch (IOException e)
            {
                if (channel.IsOpen == false)
                {
                    var custom = new ConnectionClosedException(e);
                    Logger.Info("Trying to reconnect again... Queue:" + queue, custom);
                    Thread.Sleep(200);
                    channel.BasicConsume(queue: queue, noAck: false, consumer: rabbitConsumer, consumerTag: appKey);
                }
                else
                {
                    var custom = new AccessDeniedException(e);
                    Logger.Fatal("Access denied. Queue: " + queue, custom);
                    throw custom;
                }
            }
        }

        private void EnsureRoute(string queue, string exchange, string routingKey)
        {
            try
            {
                this.channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                this.channel.ExchangeDeclare(exchange: exchange, type: "topic");
                this.channel.QueueBind(queue: queue, exchange: exchange, routingKey: routingKey);
            }
            catch (Exception e)
            {
                var custom = new UnexpectedException(e);
                Logger.Fatal("Unexpected error occured while channel create", custom);
                throw custom;
            }
        }

        public override void Dispose()
        {
            if (channel.IsClosed == false)
            {
                channel.Close();
            }
            Logger.Info("Channel closed");
        }
    }
}
