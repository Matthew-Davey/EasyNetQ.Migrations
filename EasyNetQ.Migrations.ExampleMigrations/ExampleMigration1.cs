namespace EasyNetQ.Migrations.Runner {
    [Migration(1)]
    public class ExampleMigration1 : Migration {
        public override void Apply() {
            Declare.VirtualHost("Test");

            Declare.Permission()
                .ForUser("guest")
                .OnVirtualHost("Test");
        }
    }
}