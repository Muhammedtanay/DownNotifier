using DownNotifier.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DownNotifier.Business
{
    public static class BusinessServiceExtension
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddScoped<ITargetApplicationService, TargetApplicationManager>();
            services.AddScoped<INotificationService, EmailNotificationService>();
            services.AddScoped<ILogService, LogManager>();
            return services;
        }
    }
}