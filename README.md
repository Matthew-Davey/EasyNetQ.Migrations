# EasyNetQ.Migrations
Declare your vhosts, exchanges &amp; queues in C# using a fluent interface.

[![Build Status](https://travis-ci.org/Matthew-Davey/EasyNetQ.Migrations.svg?branch=develop)](https://travis-ci.org/Matthew-Davey/EasyNetQ.Migrations) [![Nuget Downloads](https://img.shields.io/nuget/dt/EasyNetQ.Migrations.svg)](https://www.nuget.org/packages/EasyNetQ.Migrations/) [![Nuget Version](https://img.shields.io/nuget/v/EasyNetQ.Migrations.svg)](https://www.nuget.org/packages/EasyNetQ.Migrations/)

### Getting Started
* Install the latest version of EasyNetQ.Migrations from NuGet - `Install-Package EasyNetQ.Migrations`
* Declare your migrations somewhere inside your project.
* Migrations must have an incremental version number (it could also be a unix timestamp).
```
namespace MyProject.RabbitMigrations {
    [Migration(1)]
    public class ExampleMigration1 : Migration {
        public override void Apply() {
            Declare.VirtualHost("Test");

            Declare.Permission()
                .ForUser("guest")
                .OnVirtualHost("Test");
        }
    }

    [Migration(2)]
    public class ExampleMigration2 : Migration {
        public override void Apply() {
            Declare.Exchange("myExchange")
                .OnVirtualHost("Test")
                .AsType(ExchangeType.Topic)
                .Durable();

            Declare.Queue("myQueue")
                .OnVirtualHost("Test")
                .Durable();

            Declare.Binding()
                .OnVirtualHost("Test")
                .FromExchange("myExchange")
                .ToQueue("myQueue")
                .RoutingKey("#");
        }
    }

    [Migration(3)]
    public class ExampleMigration3 : Migration {
        public override void Apply() {
            Delete.Queue("myQueue")
                .OnVirtualHost("Test");

            Delete.Exchange("myExchange")
                .OnVirtualHost("Test");

            Delete.VirtualHost("Test");
        }
    }
}
```

And then run them with the included migration runner!
```
EasyNetQ.Migrations.Runner.exe -hostUrl localhost -username guest -password guest MyProject.RabbitMigrations.dll
```

_The migrations runner is located in the 'tools' folder of the Nuget package._

Outputs:
```
21:47:14.3068 Declaring vhost 'Test'
21:47:15.4943 Finished declaring vhost 'Test'
21:47:15.4943 Declaring permission for user 'guest' on vhost 'Test'
21:47:15.4943     Read Regex = .*
21:47:15.4943     WriteRegex = .*
21:47:15.4943     ConfigureRegex = .*
21:47:15.5255 Finished declaring permission for user 'guest' on vhost 'Test'
21:47:15.5412 Declaring exchange 'myExchange' on 'Test'
21:47:15.5412     Type = Topic
21:47:15.5412     AutoDelete = False
21:47:15.5412     Durable = True
21:47:15.5412     Internal = False
21:47:15.5724 Finished declaring exchange 'myExchange' on 'Test'
21:47:15.5724 Declaring queue 'myQueue' on 'Test'
21:47:15.5724     AutoDelete = False
21:47:15.5724     Durable = True
21:47:15.6037 Finished declaring queue 'myQueue' on 'Test'
21:47:15.6037 Declaring binding from 'myExchange' to 'myQueue'
21:47:15.6037     RoutingKey = #
21:47:15.6193 Finished declaring binding from 'myExchange' to 'myQueue'
21:47:15.6349 Deleting queue 'myQueue' from 'Test'
21:47:15.6506 Finished deleting queue 'myQueue' from 'Test'
21:47:15.6506 Deleting exchange 'myExchange' from 'Test'
21:47:15.6506 Finished deleting exchange 'myExchange' from 'Test'
21:47:15.6662 Deleting vhost 'Test'
21:47:15.6974 Finished Deleting vhost 'Test'

```
Successive migrations should be executed with the `-version ###` switch. In this case only migrations with a version number _greater than_ the specified value will be applied.

You can also add the `-dryrun` switch. In this case `EasyNetQ.Migrations.Runner` will log its actions but will not make any changes to your RabbitMQ broker.


### Building from source
* Requires Ruby v2.1.7 (not compatible with 2.2.3). If you need to run multiple Ruby versions use RVM (https://rvm.io/). On Windows use RubyInstaller (http://rubyinstaller.org/)
* Requires Bundler (http://bundler.io/) `gem install bundler`

```
bundler install
bundle exec rake build [configuration=Debug|Release]
```
