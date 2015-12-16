namespace EasyNetQ.Migrations.Runner {
    public class ExampleMigration : Migration {
        public override void Apply() {
            Declare.Exchange("myExchange")
                .AsType(ExchangeType.Topic)
                .Durable();

            Declare.Queue("myQueue")
                .Durable();

            Declare.Binding()
                .FromExchange("myExchange")
                .ToQueue("myQueue")
                .RoutingKey("#");
        }
    }
}