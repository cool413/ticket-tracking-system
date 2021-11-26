using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Clients.WebTicketSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
#if DEBUG
                .WriteTo.Console()
#endif
                .WriteTo.File("c:/applogs/TicketSystem.log",
                fileSizeLimitBytes: 3000000,
                rollingInterval: RollingInterval.Day,
                shared: true,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: 7,
                flushToDiskInterval: TimeSpan.FromSeconds(1))
                .CreateLogger();

            Directory.SetCurrentDirectory(AppContext.BaseDirectory);

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Stopped program because of exception");
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                 .ConfigureLogging((hostcontext, loggin) =>
                 {
                     loggin.SetMinimumLevel(LogLevel.Information);
#if DEBUG
                     loggin.AddConsole();
                     loggin.AddDebug();
#endif
                 })
                 .UseWindowsService()
                 .ConfigureAppConfiguration((hostContext, config) =>
                 {
                     Log.Logger.Information("ConfigureAppConfiguration Start");
                     var environmentName = Environment.GetEnvironmentVariable(Const.AspnetcoreEnvironment) ?? "dev";

                     if (null != args && args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
                     {
                         Log.Logger.Information($"args[0] = {args[0]}");

                         environmentName = args[0].Replace("-", "").ToLower();
                     }
                     hostContext.HostingEnvironment.EnvironmentName = environmentName;
#if DEBUG
                     environmentName = string.Empty;
                     hostContext.HostingEnvironment.EnvironmentName = "dev";
#endif
                     Log.Logger.Information($"Read appsettings.{environmentName}.json ");

                     config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                     config.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
                     config.AddEnvironmentVariables();
                     if (args != null)
                     {
                         config.AddCommandLine(args);
                     }
                 })
                 .UseSerilog((hostContext, config) =>
                 {
                     var loggerConfig = config.ReadFrom.Configuration(hostContext.Configuration);
#if DEBUG
                     loggerConfig.WriteTo.Console();
#endif
                 })
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     Log.Logger.Information("ConfigureWebHostDefaults Start");

                     var environmentName = Environment.GetEnvironmentVariable(Const.AspnetcoreEnvironment) ?? "dev";
                     var config = new ConfigurationBuilder();

#if DEBUG
                     environmentName = string.Empty;
#endif

                     if (!string.IsNullOrWhiteSpace(environmentName))
                     {
                         if (null != args && args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
                         {
                             Log.Logger.Information($"args[0] = {args[0]}");

                             environmentName = args[0].Replace("-", "").ToLower();
                         }

                         environmentName = "." + environmentName.Trim().ToLower();

                         config.AddJsonFile($"appsettings{environmentName}.json", optional: true);
                     }
                     else
                     {
                         config.AddJsonFile("appsettings.json", optional: true);
                     }

                     var configurationBuild = config.AddEnvironmentVariables()
                     .AddCommandLine(args)
                     .Build();

                     webBuilder.UseStartup<Startup>()
                        .UseEnvironment(Environment.GetEnvironmentVariable(Const.AspnetcoreEnvironment) ?? "dev")
                        .UseUrls(configurationBuild.GetSection("BaseUrl").Value);
                 })
                 .ConfigureServices((hostContext, services) =>
                 {
                     services.AddHostedService<Worker>();
                 });
    }
}
