namespace EasyNetQ.Migrations {
    using System;

    public interface IMessagesMove {
        IMessagesMove OnVirtualHost(String virtualHost);
        IMessagesMove FromQueue(String queue);
        IMessagesMove FromQueue(Type messageType, String subscriptionId);
        IMessagesMove ToExchange(String exchange);
        IMessagesMove ToExchange(Type messageType);
        IMessagesMove AsType(String messageType);
        IMessagesMove AsType(Type messageType);
    }
}