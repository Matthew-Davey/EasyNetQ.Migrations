namespace EasyNetQ.Migrations.Runner {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using EasyNetQ.Management.Client;
    using CommandLine;
    using NLog;
    using NLog.Config;
    using NLog.Layouts;
    using NLog.Targets;

    static class Program {
        static Int32 Main(String[] args) {
            var result = Parser.Default.ParseArguments<Options>(args);

            return result.MapResult(
                options => {
                    SetUpLogging(options);
                    var log = LogManager.GetCurrentClassLogger();

                    try {
                        ExecuteMigrations(options);
                    }
                    catch (Exception error) {
                        log.Error(error);
                    }
                    return 0;
                },
                errors => {
                    return 1;
                }
            );
        }

        static void SetUpLogging(Options options) {
            var configuration = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget {
                Name = "console",
                Layout = new SimpleLayout("${message}")
            };

            var consoleRule = new LoggingRule("*", LogLevel.Debug, consoleTarget);

            configuration.AddTarget(consoleTarget);
            configuration.LoggingRules.Add(consoleRule);

            if (!String.IsNullOrWhiteSpace(options.LogFilePath)) {
                var fileTarget = new FileTarget {
                    Name = "logfile",
                    CreateDirs = true,
                    FileName = options.LogFilePath,
                    Layout = new SimpleLayout("${time} ${message}")
                };

                var fileRule = new LoggingRule("*", LogLevel.Debug, fileTarget);

                configuration.AddTarget(fileTarget);
                configuration.LoggingRules.Add(fileRule);
            }

            LogManager.Configuration = configuration;
        }

        static void ExecuteMigrations(Options options) {
            var managementClient = new ManagementClient(options.HostUrl, options.Username, options.Password, options.Port);

            Assembly.LoadFrom(options.MigrationsAssemblyPath)
                .ScanForMigrationTypes()
                .VerifyVersionAttributes()
                .SkipBelowVersion(options.Version)
                .OrderByVersion()
                .InstantiateMigrations()
                .ToList()
                .ForEach(migration => {
                    if (options.DryRun) {
                        migration.DryRun();
                    }
                    else {
                        migration.Run(managementClient);
                    }
                });
        }

        static IEnumerable<Type> ScanForMigrationTypes(this Assembly assembly) {
            return assembly.GetTypes()
                .Where(typeof(Migration).IsAssignableFrom)
                .Where(type => !type.IsAbstract)
                .Where(type => !type.IsInterface);
        }

        static IEnumerable<Type> VerifyVersionAttributes(this IEnumerable<Type> migrationTypes) {
            var missingAttributes = migrationTypes.Where(type => type.GetCustomAttribute<MigrationAttribute>() == null);

            if (missingAttributes.Any()) {
                var exceptions = missingAttributes.Select(type => new Exception($"Type '{type.FullName}' is missing MigrationAttribute declaration."));
                throw new AggregateException("One or more migration types is missing MigrationAttribute declaration.", exceptions);
            }

            return migrationTypes;
        }

        static IEnumerable<Type> SkipBelowVersion(this IEnumerable<Type> migrationTypes, Int64 version) {
            return migrationTypes.Where(type => {
                var versionAttribute = type.GetCustomAttribute<MigrationAttribute>();
                return versionAttribute.Version >= version;
            });
        }

        static IOrderedEnumerable<Type> OrderByVersion(this IEnumerable<Type> migrationTypes) {
            return migrationTypes.OrderBy(type => {
                var versionAttribute = type.GetCustomAttribute<MigrationAttribute>();
                return versionAttribute.Version;
            });
        }

        static IEnumerable<Migration> InstantiateMigrations(this IEnumerable<Type> migrationTypes) {
            return migrationTypes.Select(Activator.CreateInstance)
                .Cast<Migration>();
        }
    }
}
