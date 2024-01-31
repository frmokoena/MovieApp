using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Movies.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services,
            IConfiguration configuration,
            string connectionStringName = "DefaultConnection")
        {
            services.TryAddSingleton<IDbConnectionProvider>(p =>
            {
                return new DbConnectionProvider(configuration.GetConnectionString(connectionStringName)!);
            });

            services.TryAddSingleton<IUnitOfWorkProvider, UnitOfWorkProvider>();
            return services;
        }
    }
}
