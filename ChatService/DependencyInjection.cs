using ChatService.Application.Abstractions.Auth;
using ChatService.Application.Abstractions.Behaviors;
using ChatService.Application.Abstractions.Caching;
using ChatService.Application.Abstractions.Clock;
using ChatService.Domain.Abstractions;
using ChatService.Domain.Conversations;
using ChatService.Domain.Rooms;
using ChatService.Domain.Users;
using ChatService.Infrastructure.Auth;
using ChatService.Infrastructure.Caching;
using ChatService.Infrastructure.Clock;
using ChatService.Infrastructure.Data;
using ChatService.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ChatService;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR();

        AddPersistence(services, configuration);
        AddAuthentication(services, configuration);

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

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

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();
    }

}
