namespace EasyNetQ.Migrations {
    using EasyNetQ.Management.Client;

    abstract class MigrationAction {
        protected internal abstract void VerifyState();
        protected internal abstract void DryRun();
        protected internal abstract void Apply(IManagementClient managementClient);
    }
}