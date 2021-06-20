using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApiTemplate.Services;

namespace WebApiTemplate.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Put services here
            services.AddScoped<ITodoRepository, TodoRepository>();

            return services;
        }
    }
}