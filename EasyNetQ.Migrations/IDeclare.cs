namespace EasyNetQ.Migrations {
    using System;

    public interface IDeclare {
        IExchangeDeclare Exchange(String name);
        IExchangeDeclare Exchange(Type messageType);
        IQueueDeclare Queue(String name);
        IQueueDeclare Queue(Type messageType, String subscriptionId);
        IBindingDeclare Binding();
    }
}