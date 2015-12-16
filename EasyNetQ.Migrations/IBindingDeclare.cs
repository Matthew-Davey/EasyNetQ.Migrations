namespace EasyNetQ.Migrations {
    using System;

    public interface IBindingDeclare {
        IBindingDeclare OnVirtualHost(String vhost);
        IBindingDeclare FromExchange(String name);
        IBindingDeclare FromExchange(Type messageType);
        IBindingDeclare ToQueue(String name);
        IBindingDeclare ToQueue(Type messageType, String subscriptionId);
        IBindingDeclare ToExchange(String name);
        IBindingDeclare ToExchange(Type messageType);
        IBindingDeclare RoutingKey(String routingKey);
    }
}