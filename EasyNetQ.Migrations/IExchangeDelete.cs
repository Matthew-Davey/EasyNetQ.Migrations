namespace EasyNetQ.Migrations {
    using System;

    public interface IExchangeDelete {
        IExchangeDelete OnVirtualHost(String virtualHost);
    }
}