namespace EasyNetQ.Migrations {
    using System;

    public interface IExchangeDeclare {
        IExchangeDeclare OnVirtualHost(String virtualHost);
        IExchangeDeclare AsType(ExchangeType type);
        IExchangeDeclare Durable();
        IExchangeDeclare AutoDelete();
        IExchangeDeclare Internal();
        IExchangeDeclare AlternateExchange(String alternateExchange);
    }
}