using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Services.TicketSystemService
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTicketSystemServices(this IServiceCollection services)
        {
            const string assemblyName = "Services.TicketSystemService";
            var allType = Assembly
                .Load(assemblyName)
                .GetTypes();

            allType?.ToList().ForEach(x =>
            {
                if (!x.IsGenericType && x.IsClass && x.GetInterface($"I{x.Name}", true) != null)
                {
                    services.AddScoped(x.GetInterface("I" + x.Name, true), x);
                }
            });

            return services;
        }
    }
}
