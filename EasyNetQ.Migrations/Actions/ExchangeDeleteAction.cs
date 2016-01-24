namespace EasyNetQ.Migrations.Actions {
    using System;
    using NLog;
    using Management.Client;

    class ExchangeDeleteAction : MigrationAction, IExchangeDelete {
        readonly ILogger _log = LogManager.GetCurrentClassLogger();
        public String Name { get; set; }
        public String VirtualHost { get; set; } = "/";

        public ExchangeDeleteAction(String name) {
            Name = name;
        }

        IExchangeDelete IExchangeDelete.OnVirtualHost(string virtualHost) {
            VirtualHost = virtualHost;
            return this;
        }

        public ExchangeDeleteAction(Type messageType) {
            var conventions = new Conventions(new TypeNameSerializer());
            Name = conventions.ExchangeNamingConvention(messageType);
        }

        protected internal override void VerifyState() {
            if (String.IsNullOrWhiteSpace(Name))
                throw new InvalidMigrationException("Exchange name cannot be null, empty, or whitespace.");

            if (String.IsNullOrWhiteSpace(VirtualHost))
                throw new InvalidMigrationException("VirtualHost name cannot be null, empty, or whitespace.");
        }

        protected internal override void DryRun() {
            _log.Info($"Deleting exchange '{Name}' from '{VirtualHost}'");
        }

        protected internal override void Apply(IManagementClient managementClient) {
            _log.Info($"Deleting exchange '{Name}' from '{VirtualHost}'");

            var virtualHost = managementClient.GetVhost(VirtualHost);
            var exchange = managementClient.GetExchange(Name, virtualHost);

            managementClient.DeleteExchange(exchange);

            _log.Info($"Finished deleting exchange '{Name}' from '{VirtualHost}'");
        }
    }
}