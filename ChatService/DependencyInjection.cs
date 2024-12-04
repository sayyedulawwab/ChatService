using ChatService.Domain.Abstractions;
using ChatService.Infrastructure.Data;
using ChatService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ChatService;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR();

        AddPersistence(services, configuration);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        // SQLServer
        var connectionString = configuration.GetConnectionString("SQLServer") ?? throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<EfDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<EfDbContext>());


        // MongoDB
        services.Configure<MongoDatabaseOptions>(options =>
        {
            configuration.GetSection(nameof(MongoDatabaseOptions)).Bind(options);
        });

        services.AddSingleton(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<MongoDatabaseOptions>>().Value;
            return new MongoDbContext(settings.ConnectionString, settings.DatabaseName);
        });


        // Redis
        services.AddStackExchangeRedisCache(redisOptions => {

            string connection = configuration.GetConnectionString("Redis");

            redisOptions.Configuration = connection;
        });


        services.AddSingleton<ICacheRepository, CacheRepository>();

    }

}
