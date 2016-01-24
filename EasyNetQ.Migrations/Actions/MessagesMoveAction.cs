namespace EasyNetQ.Migrations.Actions {
    using System;
    using System.Linq;
    using EasyNetQ.Management.Client;
    using EasyNetQ.Management.Client.Model;
    using NLog;

    class MessagesMoveAction : MigrationAction, IMessagesMove {
        readonly ILogger _log = LogManager.GetCurrentClassLogger();
        public String VirtualHost { get; set; }
        public String SourceQueue { get; set; }
        public String DestinationExchange { get; set; }
        public String MessageType { get; set; }

        public IMessagesMove AsType(Type messageType) {
            var typeNameSerializer = new TypeNameSerializer();
            MessageType = typeNameSerializer.Serialize(messageType);
            return this;
        }

        public IMessagesMove AsType(String messageType) {
            MessageType = messageType;
            return this;
        }

        public IMessagesMove FromQueue(String queue) {
            SourceQueue = queue;
            return this;
        }

        public IMessagesMove FromQueue(Type messageType, String subscriptionId) {
            var conventions = new Conventions(new TypeNameSerializer());
            SourceQueue = conventions.QueueNamingConvention(messageType, subscriptionId);
            return this;
        }

        public IMessagesMove OnVirtualHost(String virtualHost) {
            VirtualHost = virtualHost;
            return this;
        }

        public IMessagesMove ToExchange(String exchange) {
            DestinationExchange = exchange;
            return this;
        }

        public IMessagesMove ToExchange(Type messageType) {
            var conventions = new Conventions(new TypeNameSerializer());
            DestinationExchange = conventions.ExchangeNamingConvention(messageType);
            return this;
        }

        Boolean VirtualHostDeclared => !String.IsNullOrWhiteSpace(VirtualHost);
        Boolean SourceQueueDeclared => !String.IsNullOrWhiteSpace(SourceQueue);
        Boolean DestinationExchangeDeclared => !String.IsNullOrWhiteSpace(DestinationExchange);
        Boolean MessageTypeDeclared => !String.IsNullOrWhiteSpace(MessageType);

        protected internal override void VerifyState() {
            if (!VirtualHostDeclared)
                throw new InvalidMigrationException("VirtualHost name cannot be null, empty, or whitespace.");

            if (!SourceQueueDeclared)
                throw new InvalidMigrationException("Source queue name cannot be null, empty, or whitespace.");

            if (!DestinationExchangeDeclared)
                throw new InvalidMigrationException("Destination exchange name cannot be null, empty, or whitespace.");
        }

        protected internal override void DryRun() {
            _log.Info($"Moving messages from queue '{SourceQueue}' to exchange '{DestinationExchange}'");
            if (MessageTypeDeclared)
                _log.Info($"    MessageType = {MessageType}");
        }

        protected internal override void Apply(IManagementClient managementClient) {
            _log.Info($"Moving messages from queue '{SourceQueue}' to exchange '{DestinationExchange}'");
            if (MessageTypeDeclared)
                _log.Info($"    MessageType = {MessageType}");

            var virtualHost = managementClient.GetVhost(VirtualHost);
            var sourceQueue = managementClient.GetQueue(SourceQueue, virtualHost);
            var destinationExchange = managementClient.GetExchange(DestinationExchange, virtualHost);

            var messages = managementClient.GetMessagesFromQueue(sourceQueue, new GetMessagesCriteria(long.MaxValue, false));

            foreach (var message in messages) {
                if (MessageTypeDeclared) {
                    message.Properties["type"] = MessageType;
                }

                var properties = message.Properties
                    .ToDictionary(property => property.Key, property => property.Value);

                managementClient.Publish(destinationExchange, new PublishInfo(properties, "#", message.Payload, message.PayloadEncoding));
            }
        }
    }
}