namespace EasyNetQ.Migrations.Actions {
    using System;
    using NLog;
    using Management.Client;
    using Management.Client.Model;

    class QueueDeclareAction : MigrationAction, IQueueDeclare {
        readonly ILogger _log = LogManager.GetCurrentClassLogger();
        public String Name { get; set; }
        public String VirtualHost { get; set; } = "/";
        public Boolean AutoDelete { get; set; }
        public TimeSpan? AutoExpire { get; set; }
        public String DeadLetterExchange { get; set; }
        public String DeadLetterRoutingKey { get; set; }
        public Boolean Durable { get; set; }
        public Int32? MaximumPriority { get; set; }
        public Int64? MaxLength { get; set; }
        public Int64? MaxLengthBytes { get; set; }
        public TimeSpan? MessageTTL { get; set; }

        public QueueDeclareAction(String name) {
            Name = name;
        }

        public QueueDeclareAction(Type messageType, String subscriptionId) {
            var conventions = new Conventions(new TypeNameSerializer());
            Name = conventions.QueueNamingConvention(messageType, subscriptionId);
        }

        IQueueDeclare IQueueDeclare.AutoDelete() {
            AutoDelete = true;
            return this;
        }

        IQueueDeclare IQueueDeclare.AutoExpire(TimeSpan autoExpire) {
            AutoExpire = autoExpire;
            return this;
        }

        IQueueDeclare IQueueDeclare.DeadLetterExchange(String dlx) {
            DeadLetterExchange = dlx;
            return this;
        }

        IQueueDeclare IQueueDeclare.DeadLetterExchange(String dlx, String routingKey) {
            DeadLetterExchange = dlx;
            DeadLetterRoutingKey = routingKey;
            return this;
        }

        IQueueDeclare IQueueDeclare.Durable() {
            Durable = true;
            return this;
        }

        IQueueDeclare IQueueDeclare.MaximumPriority(Int32 maxPriority) {
            MaximumPriority = maxPriority;
            return this;
        }

        IQueueDeclare IQueueDeclare.MaxLength(Int64 maxLength) {
            MaxLength = maxLength;
            return this;
        }

        IQueueDeclare IQueueDeclare.MaxLengthBytes(Int64 maxLength) {
            MaxLengthBytes = maxLength;
            return this;
        }

        IQueueDeclare IQueueDeclare.MessageTTL(TimeSpan ttl) {
            MessageTTL = ttl;
            return this;
        }

        IQueueDeclare IQueueDeclare.OnVirtualHost(String vhost) {
            VirtualHost = vhost;
            return this;
        }

        Boolean MessageTTLDeclared     => MessageTTL.HasValue;
        Boolean AutoExpireDeclared     => AutoExpire.HasValue;
        Boolean MaxLengthDeclared      => MaxLength.HasValue;
        Boolean MaxLengthBytesDeclared => MaxLengthBytes.HasValue;
        Boolean DLXDeclared            => !String.IsNullOrWhiteSpace(DeadLetterExchange);
        Boolean DLXRoutingKeyDeclared  => !String.IsNullOrWhiteSpace(DeadLetterRoutingKey);
        Boolean MaxPriorityDeclared    => MaximumPriority.HasValue;

        protected internal override void VerifyState() {
            if (String.IsNullOrWhiteSpace(Name))
                throw new InvalidMigrationException("Exchange name cannot be null, empty, or whitespace.");

            if (String.IsNullOrWhiteSpace(VirtualHost))
                throw new InvalidMigrationException("VirtualHost name cannot be null, empty, or whitespace.");
        }

        protected internal override void Apply(IManagementClient managementClient) {
            _log.Info($"Declaring queue '{Name}' on '{VirtualHost}'");
            _log.Info($"    AutoDelete = {AutoDelete}");
            _log.Info($"    Durable = {Durable}");

            var vhost = managementClient.GetVhost(VirtualHost);
            var arguments = new InputArguments();
            var queueInfo = new QueueInfo(Name, AutoDelete, Durable, arguments);

            if (MessageTTLDeclared) {
                _log.Info($"    MessageTTL = {MessageTTL}");
                arguments.Add("x-message-ttl", (long)MessageTTL.Value.TotalMilliseconds);
            }
            if (AutoExpireDeclared) {
                _log.Info($"    AutoExpire = {AutoExpire}");
                arguments.Add("x-expires", (long)AutoExpire.Value.TotalMilliseconds);
            }
            if (MaxLengthDeclared) {
                _log.Info($"    MaxLength = {MaxLength}");
                arguments.Add("x-max-length", MaxLength.Value);
            }
            if (MaxLengthBytesDeclared) {
                _log.Info($"    MaxLengthBytes = {MaxLengthBytes}");
                arguments.Add("x-max-length-bytes", MaxLengthBytes.Value);
            }
            if (DLXDeclared) {
                _log.Info($"    DeadLetterExchange = {DeadLetterExchange}");
                arguments.Add("x-dead-letter-exchange", DeadLetterExchange);
            }
            if (DLXRoutingKeyDeclared) {
                _log.Info($"    DeadLetterRoutingKey = {DeadLetterRoutingKey}");
                arguments.Add("x-dead-letter-routing-key", DeadLetterRoutingKey);
            }
            if (MaxPriorityDeclared) {
                _log.Info($"    MaxPriority = {MaximumPriority}");
                arguments.Add("x-max-priority", MaximumPriority.Value);
            }
            
            managementClient.CreateQueue(queueInfo, vhost);

            _log.Info($"Finished declaring queue '{Name}' on '{VirtualHost}'");
        }
    }
}