namespace EasyNetQ.Migrations {
    using System;
    using System.Collections.Generic;
    using EasyNetQ.Migrations.Actions;
    using EasyNetQ.Management.Client;

    public abstract class Migration : IDeclare {
        readonly List<MigrationAction> _actions = new List<MigrationAction>();

        public IDeclare Declare => this;

        IBindingDeclare IDeclare.Binding() {
            var bindingDeclare = new BindingDeclareAction();
            _actions.Add(bindingDeclare);
            return bindingDeclare;
        }

        IExchangeDeclare IDeclare.Exchange(Type messageType) {
            var exchangeDeclare = new ExchangeDeclareAction(messageType);
            _actions.Add(exchangeDeclare);
            return exchangeDeclare;
        }

        IExchangeDeclare IDeclare.Exchange(String name) {
            var exchangeDeclare = new ExchangeDeclareAction(name);
            _actions.Add(exchangeDeclare);
            return exchangeDeclare;
        }

        IQueueDeclare IDeclare.Queue(String name) {
            var queueDeclare = new QueueDeclareAction(name);
            _actions.Add(queueDeclare);
            return queueDeclare;
        }

        IQueueDeclare IDeclare.Queue(Type messageType, String subscriptionId) {
            var queueDeclare = new QueueDeclareAction(messageType, subscriptionId);
            _actions.Add(queueDeclare);
            return queueDeclare;
        }

        public abstract void Apply();

        public void Run(IManagementClient managementClient) {
            Apply();

            _actions.ForEach(action => action.VerifyState());
            _actions.ForEach(action => action.Apply(managementClient));
        }
    }
}