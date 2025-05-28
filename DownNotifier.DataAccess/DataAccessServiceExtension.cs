using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DownNotifier.DataAccess.Concrete;
using DownNotifier.DataAccess.Abstract;

namespace DownNotifier.DataAccess
{
    public static class DataAccessServiceExtension
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DownNotifierDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DownNotifierDbContext")));

            services.AddScoped<ITargetApplicationRepository, TargetApplicationRepository>();
            services.AddScoped<ILogRepository, LogRepository>();

            return services;
        }

    }
}
