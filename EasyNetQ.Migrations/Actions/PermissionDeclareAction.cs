namespace EasyNetQ.Migrations.Actions {
    using System;
    using Management.Client;
    using Management.Client.Model;
    using NLog;

    class PermissionDeclareAction : MigrationAction, IPermissionDeclare {
        readonly ILogger _log = LogManager.GetCurrentClassLogger();
        public String User { get; set; }
        public String VirtualHost { get; set; }
        public String ConfigureRegex { get; set; } = ".*";
        public String WriteRegex { get; set; } = ".*";
        public String ReadRegex { get; set; } = ".*";

        IPermissionDeclare IPermissionDeclare.ForUser(string username) {
            User = username;
            return this;
        }

        IPermissionDeclare IPermissionDeclare.OnVirtualHost(string virtualHost) {
            VirtualHost = virtualHost;
            return this;
        }

        IPermissionDeclare IPermissionDeclare.Configure(string configureRegex) {
            ConfigureRegex = configureRegex;
            return this;
        }

        IPermissionDeclare IPermissionDeclare.Write(string writeRegex) {
            WriteRegex = writeRegex;
            return this;
        }

        IPermissionDeclare IPermissionDeclare.Read(string readRegex) {
            ReadRegex = readRegex;
            return this;
        }

        Boolean UserDeclared => !String.IsNullOrWhiteSpace(User);
        Boolean VirtualHostDeclared => !String.IsNullOrWhiteSpace(VirtualHost);
        Boolean ConfigureRegexDeclared => !String.IsNullOrWhiteSpace(ConfigureRegex);
        Boolean WriteRegexDeclared => !String.IsNullOrWhiteSpace(WriteRegex);
        Boolean ReadRegexDeclared => !String.IsNullOrWhiteSpace(ReadRegex);

        protected internal override void VerifyState() {
            if (!UserDeclared)
                throw new InvalidMigrationException("User name cannot be null, empty, or whitespace.");

            if (!VirtualHostDeclared)
                throw new InvalidMigrationException("VirtualHost name cannot be null, empty, or whitespace.");
        }

        protected internal override void Apply(IManagementClient managementClient) {
            _log.Info($"Declaring permission for user '{User}' on vhost '{VirtualHost}'");
            _log.Info($"    Read Regex = {ReadRegex}");
            _log.Info($"    WriteRegex = {WriteRegex}");
            _log.Info($"    ConfigureRegex = {ConfigureRegex} ");

            var user = managementClient.GetUser(User);
            var virtualHost = managementClient.GetVhost(VirtualHost);
            var permissionInfo = new PermissionInfo(user, virtualHost);
            permissionInfo.SetConfigure(ConfigureRegex);
            permissionInfo.SetRead(ReadRegex);
            permissionInfo.SetWrite(WriteRegex);

            managementClient.CreatePermission(permissionInfo);

            _log.Info($"Finished declaring permission for user '{User}' on vhost '{VirtualHost}'");
        }
    }
}