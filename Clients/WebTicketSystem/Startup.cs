using Databases.TicketSystemContext;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Infrastructure.Extensions;
using Services.TicketSystemService;

namespace Clients.WebTicketSystem
{
    public class Startup
    {
        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            var ticketSystemConnectionString = Configuration.GetConnectionString("TicketSystemConnectionString");
            
            services.AddOptions();
            services.AddLogging();

            services.AddEFSecondLevelCache(option =>
            {
#if DEBUG
                option.DisableLogging(false);
#else
                option.DisableLogging(true);
#endif
                option.UseMemoryCacheProvider();
            });
            services.AddEntityFrameworkSqlServer();
            services.AddTicketSystemRespository(ticketSystemConnectionString);
            services.AddTicketSystemServices();
            
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins, builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                    //var result = new BadRequestObjectResult(context.ModelState);

                    //// TODO: add `using System.Net.Mime;` to resolve MediaTypeNames
                    //result.ContentTypes.Add(MediaTypeNames.Application.Json);
                    //result.ContentTypes.Add(MediaTypeNames.Application.Xml);
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
            
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCustomExceptionMiddleware();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}