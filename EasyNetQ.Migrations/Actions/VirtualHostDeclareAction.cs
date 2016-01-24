namespace EasyNetQ.Migrations.Actions {
    using System;
    using Management.Client;
    using NLog;

    class VirtualHostDeclareAction : MigrationAction {
        readonly ILogger _log = LogManager.GetCurrentClassLogger();
        public String Name { get; set; }

        public VirtualHostDeclareAction(String name) {
            Name = name;
        }

        protected internal override void VerifyState() {
            if (String.IsNullOrWhiteSpace(Name))
                throw new InvalidMigrationException("Name cannot be null, empty, or whitespace");
        }

        protected internal override void DryRun() {
            _log.Info($"Declaring vhost '{Name}'");
        }

        protected internal override void Apply(IManagementClient managementClient) {
            _log.Info($"Declaring vhost '{Name}'");
            managementClient.CreateVirtualHost(Name);
            _log.Info($"Finished declaring vhost '{Name}'");
        }
    }
}