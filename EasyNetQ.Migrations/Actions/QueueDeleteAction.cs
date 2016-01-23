namespace EasyNetQ.Migrations.Actions {
    using System;
    using NLog;
    using Management.Client;

    class QueueDeleteAction : MigrationAction, IQueueDelete {
        readonly ILogger _log = LogManager.GetCurrentClassLogger();
        public String Name { get; set; }
        public String VirtualHost { get; set; } = "/";

        public QueueDeleteAction(String name) {
            Name = name;
        }

        public QueueDeleteAction(Type messageType, String subscriptionId) {
            var conventions = new Conventions(new TypeNameSerializer());
            Name = conventions.QueueNamingConvention(messageType, subscriptionId);
        }

        IQueueDelete IQueueDelete.OnVirtualHost(String virtualHost) {
            VirtualHost = virtualHost;
            return this;
        }

        protected internal override void VerifyState() {
            if (String.IsNullOrWhiteSpace(Name))
                throw new InvalidMigrationException("Exchange name cannot be null, empty, or whitespace.");

            if (String.IsNullOrWhiteSpace(VirtualHost))
                throw new InvalidMigrationException("VirtualHost name cannot be null, empty, or whitespace.");
        }

        protected internal override void Apply(IManagementClient managementClient) {
            _log.Info($"Deleting queue '{Name}' from '{VirtualHost}'");

            var virtualHost = managementClient.GetVhost(VirtualHost);
            var queue = managementClient.GetQueue(Name, virtualHost);

            managementClient.DeleteQueue(queue);

            _log.Info($"Finished deleting queue '{Name}' from '{VirtualHost}'");
        }
    }
}