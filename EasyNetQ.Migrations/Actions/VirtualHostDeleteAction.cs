namespace EasyNetQ.Migrations.Actions {
    using System;
    using Management.Client;
    using NLog;

    class VirtualHostDeleteAction : MigrationAction {
        readonly ILogger _log = LogManager.GetCurrentClassLogger();
        public String Name { get; set; }

        public VirtualHostDeleteAction(String name) {
            Name = name;
        }

        protected internal override void VerifyState() {
            if (String.IsNullOrWhiteSpace(Name))
                throw new InvalidMigrationException("Name cannot be null, empty, or whitespace");
        }

        protected internal override void DryRun() { 
            _log.Info($"Deleting vhost '{Name}'");
        }

        protected internal override void Apply(IManagementClient managementClient) {
            _log.Info($"Deleting vhost '{Name}'");

            var virtualHost = managementClient.GetVhost(Name);
            managementClient.DeleteVirtualHost(virtualHost);

            _log.Info($"Finished Deleting vhost '{Name}'");
        }
    }
}