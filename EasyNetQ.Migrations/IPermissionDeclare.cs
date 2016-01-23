namespace EasyNetQ.Migrations {
    using System;

    public interface IPermissionDeclare {
        IPermissionDeclare ForUser(String username);
        IPermissionDeclare OnVirtualHost(String virtualHost);
        IPermissionDeclare Configure(String configureRegex);
        IPermissionDeclare Write(String writeRegex);
        IPermissionDeclare Read(String readRegex);
    }
}