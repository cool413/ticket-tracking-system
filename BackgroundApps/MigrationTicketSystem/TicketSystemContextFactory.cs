using System;
using System.Linq;
using System.Reflection;
using Databases.TicketSystemContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using Services.DataCommon;

namespace BackgroundApps.MigrationTicketSystem
{
    public sealed class TicketSystemContextFactory : IDesignTimeDbContextFactory<TicketSystemDbContext>
    {
        internal string ConnectionString { private get; set; }

        public TicketSystemDbContext CreateDbContext(string[] args)
        {
            ResourceHelper.Initial(Assembly.GetExecutingAssembly(),
                "BackgroundApps.MigrationTicketSystem.Scripts");

            if (string.IsNullOrEmpty(ConnectionString))
            {
                ConnectionString = Program.GetConnectionStrings()?.FirstOrDefault().Value;
            }

            var optionsBuilder = new DbContextOptionsBuilder<TicketSystemDbContext>()
                .ReplaceService<IMigrationsSqlGenerator, MigrationsGenerator>()
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .UseSqlServer(ConnectionString, opts =>
                {
                    opts.CommandTimeout((int)TimeSpan.FromSeconds(2400).TotalSeconds);
                    opts.EnableRetryOnFailure(2);
                    opts.MigrationsHistoryTable("_EFMigrationsHistory", "dbo");
                    opts.MigrationsAssembly("BackgroundApps.MigrationTicketSystem");
                });

            return new TicketSystemDbContext(optionsBuilder.Options);
        }
    }
}