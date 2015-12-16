namespace EasyNetQ.Migrations.Runner {
    public class ExampleMigration2 : Migration {
        public override void Apply() {
            Delete.Queue("myQueue");
            Delete.Exchange("myExchange");
        }
    }
}