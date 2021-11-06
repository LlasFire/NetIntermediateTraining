using RabbitMQ.Client;

namespace Infrastructure
{
    public static class MessageClient
    {
        public static IModel GenerateFileChannel()
        {
            var channel = GenerateConnection();

            channel.QueueDeclare(queue: "hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
            return channel;
        }

        public static IModel GenerateHeathCheckChannel()
        {
            var channel = GenerateConnection();

            channel.QueueDeclare(queue: "healthcheck",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
            return channel;
        }

        public static (IModel, string) GenerateSettingsChannel()
        {
            var channel = GenerateConnection();

            channel.ExchangeDeclare(exchange: "settings", type: ExchangeType.Fanout);
            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName,
                              exchange: "settings",
                              routingKey: "");
            return (channel, queueName);
        }

        private static IModel GenerateConnection()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            return channel;
        }
    }
}
