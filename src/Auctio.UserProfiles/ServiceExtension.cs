using Auctio.UserProfiles.Consumers;
using Auctio.Shared.Masstransit;
using MassTransit;
using Npgsql;

namespace Auctio.UserProfiles;

public static class ServiceExtension
{
    public static IServiceCollection ConfigureDbSource(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddNpgsqlDataSource(connectionString!);
        
        var dataSource = services.BuildServiceProvider().GetRequiredService<NpgsqlDataSource>();
        
        using (var connection = dataSource.CreateConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS user_profiles(
                        id TEXT PRIMARY KEY NOT NULL,
                        name VARCHAR(100) UNIQUE NOT NULL,
                        createdAt TIMESTAMP,
                        bio TEXT
                    );
                ";
                command.ExecuteNonQuery();
            }
        }
        return services;
    }
    public static IServiceCollection ConfigureConsumers(this IServiceCollection services)
    {
        services.AddMassTransit(x=>
        {
            x.AddConsumer<CreateUserConsumer>();
            x.UsingRabbitMq((context, cfg) => 
            {
                cfg.Host(CreateUserQueueConfig.Host, c => 
                {
                    c.Username(CreateUserQueueConfig.Username);
                    c.Password(CreateUserQueueConfig.Password);
                });
                cfg.ReceiveEndpoint("CreateUserQueue", e =>
                {
                    e.ConfigureConsumer<CreateUserConsumer>(context);
                });
            });
            
        });
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<Repositories.UserProfileRepository>();
        services.AddSingleton<Services.UserProfileCacheService>();
        services.AddScoped<Services.UserProfileService>();
        return services;
    }

    public static IServiceCollection ConfigureCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "UserProfileCache_";
        });
        return services;
    }

}