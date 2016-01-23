# EasyNetQ.Migrations
Declare your vhosts, exchanges &amp; queues in C# using a fluent interface.

```
namespace MyProject.RabbitMigrations {
    public class MyMigration1 : Migration {
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
    
    public class MyMigration2 : Migration {
        public override void Apply() {
            Delete.Queue("myQueue");
            Delete.Exchange("myExchange");
        }
    }
}
```
