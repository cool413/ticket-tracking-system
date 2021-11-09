using System.Linq;
using System.Reflection;
using Databases.TicketSystemContext.Repositories;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.DataCommon;

namespace Databases.TicketSystemContext
{
     public static class ServiceExtensions
    {
        public static IServiceCollection AddTicketSystemRespository(this IServiceCollection services,
         string connectionString)
        {
            services.AddTicketSystemDbContext(connectionString);

            const string assemblyName = "Databases.TicketSystemContext";
            var allType = Assembly
                .Load(assemblyName)
                .GetTypes();

            allType?.ToList().ForEach(x =>
            {
                if (!x.IsGenericType && x.IsClass && x.GetInterface($"I{x.Name}", true) != null)
                {
                    services.AddScoped(x.GetInterface("I" + x.Name, true), x);
                }
                else if (x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ITicketSystemBaseRepository<>) && !x.IsAbstract)
                {
                    var serviceType = x.GetInterfaces().First(j => j.GetGenericTypeDefinition() == typeof(TicketSystemBaseRepository<>));
                    services.AddScoped(serviceType, x);
                }
            });

            return services;
        }

        public static IServiceCollection AddTicketSystemDbContext(this IServiceCollection services,
        string connectionString)
        {
            services.AddEFSecondLevelCache(options =>
               options.UseCacheManagerCoreProvider().DisableLogging(true)
           );

            services.AddDbContextPool<TicketSystemDbContext>((serviceProvider, option) =>
            {
#if DEBUG
                option.UseLoggerFactory(LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                }));
                option.EnableSensitiveDataLogging(true);
#endif
                option.UseSqlServer(connectionString, serverOption =>
                {
                    serverOption.CommandTimeout(180);
                    serverOption.EnableRetryOnFailure(3);
                });

                option.AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());

                CustomDbCommandInterceptor.EnableRecompile.Value = false;
                CustomDbCommandInterceptor.EnableNolock.Value = false;
                option.AddInterceptors(new CustomDbCommandInterceptor());
            }, 128);
            return services;
        }
    }
}
