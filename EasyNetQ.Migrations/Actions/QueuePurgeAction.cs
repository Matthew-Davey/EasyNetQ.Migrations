namespace EasyNetQ.Migrations {
    using System;
    using NLog;
    using Management.Client;

    class QueuePurgeAction : MigrationAction, IQueuePurge {
        readonly ILogger _log = LogManager.GetCurrentClassLogger();
        public String Name { get; set; }
        public String VirtualHost { get; set; }

        public QueuePurgeAction(String name) {
            Name = name;
        }

        public QueuePurgeAction(Type messageType, String subscriptionId) {
            var conventions = new Conventions(new TypeNameSerializer());
            Name = conventions.QueueNamingConvention(messageType, subscriptionId);
        }

        IQueuePurge IQueuePurge.OnVirtualHost(String virtualHost) {
            VirtualHost = virtualHost;
            return this;
        }

        protected internal override void VerifyState() {
            if (String.IsNullOrWhiteSpace(Name))
                throw new InvalidMigrationException("Exchange name cannot be null, empty, or whitespace.");

            if (String.IsNullOrWhiteSpace(VirtualHost))
                throw new InvalidMigrationException("VirtualHost name cannot be null, empty, or whitespace.");
        }

        protected internal override void DryRun() {
            _log.Info($"Purging queue '{Name}' on '{VirtualHost}'");
        }

        protected internal override void Apply(IManagementClient managementClient) {
            _log.Info($"Purging queue '{Name}' on '{VirtualHost}'");

            var virtualHost  = managementClient.GetVhost(VirtualHost);
            var queue = managementClient.GetQueue(Name, virtualHost);

            managementClient.Purge(queue);

            _log.Info($"Finished purging queue '{Name}' on '{VirtualHost}'");
        }
    }
}