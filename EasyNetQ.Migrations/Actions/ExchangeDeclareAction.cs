namespace EasyNetQ.Migrations.Actions {
    using System;
    using NLog;
    using Management.Client;
    using Management.Client.Model;
    using ExchangeType = ExchangeType;

    class ExchangeDeclareAction : MigrationAction, IExchangeDeclare {
        readonly ILogger _log = LogManager.GetCurrentClassLogger();
        public String Name { get; set; }
        public String VirtualHost { get; set; } = "/";
        public ExchangeType Type { get; set; } = ExchangeType.Topic;
        public Boolean Durable { get; set; }
        public Boolean AutoDelete { get; set; }
        public Boolean Internal { get; set; }
        public String AlternateExchange { get; set; }

        public ExchangeDeclareAction(String name) {
            Name = name;
        }

        public ExchangeDeclareAction(Type messageType) {
            var conventions = new Conventions(new TypeNameSerializer());
            Name = conventions.ExchangeNamingConvention(messageType);
        }

        IExchangeDeclare IExchangeDeclare.AlternateExchange(String alternateExchange) {
            AlternateExchange = alternateExchange;
            return this;
        }

        IExchangeDeclare IExchangeDeclare.AsType(ExchangeType type) {
            Type = type;
            return this;
        }

        IExchangeDeclare IExchangeDeclare.AutoDelete() {
            AutoDelete = true;
            return this;
        }

        IExchangeDeclare IExchangeDeclare.Durable() {
            Durable = true;
            return this;
        }

        IExchangeDeclare IExchangeDeclare.Internal() {
            Internal = true;
            return this;
        }

        IExchangeDeclare IExchangeDeclare.OnVirtualHost(String virtualHost) {
            VirtualHost = virtualHost;
            return this;
        }

        protected internal override void VerifyState() {
            if (String.IsNullOrWhiteSpace(Name))
                throw new InvalidMigrationException("Exchange name cannot be null, empty, or whitespace.");

            if (String.IsNullOrWhiteSpace(VirtualHost))
                throw new InvalidMigrationException("VirtualHost name cannot be null, empty, or whitespace.");

            if (!Enum.IsDefined(typeof(ExchangeType), Type))
                throw new InvalidMigrationException("Exchange type is not a valid value.");
        }

        protected internal override void DryRun() {
            _log.Info($"Declaring exchange '{Name}' on '{VirtualHost}'");
            _log.Info($"    Type = {Type}");
            _log.Info($"    AutoDelete = {AutoDelete}");
            _log.Info($"    Durable = {Durable}");
            _log.Info($"    Internal = {Internal}");

            if (AlternateExchangeDeclared) {
                _log.Info($"    AlternateExchange = {AlternateExchange}");
            }
        }

        Boolean AlternateExchangeDeclared => !String.IsNullOrWhiteSpace(AlternateExchange);

        protected internal override void Apply(IManagementClient managementClient) {
            _log.Info($"Declaring exchange '{Name}' on '{VirtualHost}'");
            _log.Info($"    Type = {Type}");
            _log.Info($"    AutoDelete = {AutoDelete}");
            _log.Info($"    Durable = {Durable}");
            _log.Info($"    Internal = {Internal}");

            var virtualHost  = managementClient.GetVhost(VirtualHost);
            var exchangeType = Type.ToString().ToLowerInvariant();
            var arguments    = new Arguments();
            var exchangeInfo = new ExchangeInfo(Name, exchangeType, AutoDelete, Durable, Internal, arguments);

            if (AlternateExchangeDeclared) {
                _log.Info($"    AlternateExchange = {AlternateExchange}");
                arguments.Add("alternate-exchange", AlternateExchange);
            }

            managementClient.CreateExchange(exchangeInfo, virtualHost);

            _log.Info($"Finished declaring exchange '{Name}' on '{VirtualHost}'");
        }
    }
}