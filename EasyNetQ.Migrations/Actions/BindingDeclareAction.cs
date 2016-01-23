namespace EasyNetQ.Migrations.Actions {
    using System;
    using NLog;
    using Management.Client;
    using Management.Client.Model;

    class BindingDeclareAction : MigrationAction, IBindingDeclare {
        readonly ILogger _log = LogManager.GetCurrentClassLogger();
        public String FromExchange { get; set; }
        public String VirtualHost { get; set; } = "/";
        public String RoutingKey { get; set; } = "#";
        public String ToExchange { get; set; }
        public String ToQueue { get; set; }

        IBindingDeclare IBindingDeclare.FromExchange(Type messageType) {
            var conventions = new Conventions(new TypeNameSerializer());
            FromExchange = conventions.ExchangeNamingConvention(messageType);
            return this;
        }

        IBindingDeclare IBindingDeclare.FromExchange(String name) {
            FromExchange = name;
            return this;
        }

        IBindingDeclare IBindingDeclare.OnVirtualHost(String vhost) {
            VirtualHost = vhost;
            return this;
        }

        IBindingDeclare IBindingDeclare.RoutingKey(String routingKey) {
            RoutingKey = routingKey;
            return this;
        }

        IBindingDeclare IBindingDeclare.ToExchange(Type messageType) {
            var conventions = new Conventions(new TypeNameSerializer());
            ToExchange = conventions.ExchangeNamingConvention(messageType);
            return this;
        }

        IBindingDeclare IBindingDeclare.ToExchange(String name) {
            ToExchange = name;
            return this;
        }

        IBindingDeclare IBindingDeclare.ToQueue(String name) {
            ToQueue = name;
            return this;
        }

        IBindingDeclare IBindingDeclare.ToQueue(Type messageType, String subscriptionId) {
            var conventions = new Conventions(new TypeNameSerializer());
            ToQueue = conventions.QueueNamingConvention(messageType, subscriptionId);
            return this;
        }

        Boolean ToQueueDeclared => !String.IsNullOrWhiteSpace(ToQueue);
        Boolean ToExchangeDeclared => !String.IsNullOrWhiteSpace(ToExchange);

        protected internal override void VerifyState() {
            if (String.IsNullOrWhiteSpace(VirtualHost))
                throw new InvalidMigrationException("VirtualHost name cannot be null, empty, or whitespace.");

            if (String.IsNullOrWhiteSpace(FromExchange))
                throw new InvalidMigrationException("FromExchange cannot be null, empty, or whitespace.");

            if (String.IsNullOrWhiteSpace(RoutingKey))
                throw new InvalidMigrationException("RoutingKey cannot be null, empty, or whitespace.");

            if (String.IsNullOrWhiteSpace(ToExchange) && String.IsNullOrWhiteSpace(ToQueue))
                throw new InvalidMigrationException("Must specify either ToExchange or ToQueue.");

            if (!String.IsNullOrWhiteSpace(ToExchange) && !String.IsNullOrWhiteSpace(ToQueue))
                throw new InvalidMigrationException("Must specify either ToExchange or ToQueue.");
        }

        protected internal override void Apply(IManagementClient managementClient) {
            var destinationName = ToQueueDeclared ? ToQueue : ToExchange;

            _log.Info($"Declaring binding from '{FromExchange}' to '{destinationName}'");
            _log.Info($"    RoutingKey = {RoutingKey}");

            var virtualHost = managementClient.GetVhost(VirtualHost);
            var exchange = managementClient.GetExchange(FromExchange, virtualHost);
            var info = new BindingInfo(RoutingKey);

            if (ToQueueDeclared) {
                var toQueue = managementClient.GetQueue(ToQueue, virtualHost);
                managementClient.CreateBinding(exchange, toQueue, info);
            }

            if (ToExchangeDeclared) {
                var toExchange = managementClient.GetExchange(ToExchange, virtualHost);
                managementClient.CreateBinding(exchange, toExchange, info);
            }

            _log.Info($"Finished declaring binding from '{FromExchange}' to '{destinationName}'");
        }
    }
}