using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BackgroundApps.MigrationTicketSystem
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";
            var AppName = typeof(Program).Namespace;
            Console.WriteLine($"==Database: {AppName} ==");
            Console.WriteLine($"==EnvVar: {environmentName}==");

            var factory = new TicketSystemContextFactory();
            var result = true;

            var Conns = GetConnectionStrings(environmentName).ToList();
            if (args != null && args.Length >= 1 && !string.IsNullOrEmpty(args[0]))
            {
                Conns = Conns.Where(x => x.Key.IndexOf(args[0]) > -1).ToList();
            }

            if (Conns.Count <= 0)
            {
                Console.WriteLine($"{AppName} Connection does't find in appsettings.json");
                return 0;
            }

            Conns.ToList().ForEach(c =>
            {
                Console.WriteLine($"ConnKey: {c.Key},ConnStr: {c.Value}");
                var ConnStr = c.Value;
                factory.ConnectionString = ConnStr;

                using (var context = factory.CreateDbContext(args))
                {
                    try
                    {
                        var db = context.Database;
                        db.Migrate();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{c.Value} {ex.Message}");
                        result = false;
                        return;
                    }

#if SEED
                    var ResourceNames = ResourceHelper.GetAllSqls("Seed");
                    foreach (var ResourceName in ResourceNames)
                        executeSqlFile(ResourceHelper.GetSql(ResourceName), ConnStr);
#endif
                }
            });
            return result ? 0 : 1;
        }

        internal static IEnumerable<IConfigurationSection> GetConnectionStrings(string environmentName = "")
        {
            var configurationBuilder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddEnvironmentVariables();

#if SEED || DEBUG
            environmentName = string.Empty;
#endif

            if (!string.IsNullOrWhiteSpace(environmentName))
            {
                environmentName = "." + environmentName.Trim().ToLower();
                configurationBuilder.AddJsonFile($"appsettings{environmentName}.json", optional: true);
            }
            else
            {
                configurationBuilder.AddJsonFile("appsettings.json", optional: true);
            }

            return configurationBuilder.Build().GetSection("ConnectionStrings").GetChildren();
        }

        internal static void executeSqlFile(string script, string connStr)
        {
            // split script on GO command
            var commandStrings = Regex.Split(script, @"^\s*GO\s*$",
                                     RegexOptions.Multiline | RegexOptions.IgnoreCase);

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                foreach (string commandString in commandStrings)
                {
                    if (commandString.Trim() == "")
                    {
                        continue;
                    }

                    using (var command = new SqlCommand(commandString, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
