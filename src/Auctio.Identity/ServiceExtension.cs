using Auctio.Identity.Services;
using MassTransit;
using Npgsql;
using Auctio.Shared.Masstransit;
using MassTransit.Internals.Caching;


namespace Auctio.Identity;

public static class ServiceExtension
{
    public static IServiceCollection ConfigureDbSource(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddNpgsqlDataSource(connectionString!);
        
        // Create the table if it doesn't exist
        var dataSource = services.BuildServiceProvider().GetRequiredService<NpgsqlDataSource>();
        
        using (var connection = dataSource.CreateConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";
                    CREATE TABLE IF NOT EXISTS users(
                        id TEXT PRIMARY KEY DEFAULT uuid_generate_v4(),
                        name VARCHAR(100) UNIQUE NOT NULL,
                        createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        passwordHash TEXT NOT NULL
                    );
                ";
                command.ExecuteNonQuery();
            }
        }
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IdentityProvider>();
        services.AddSingleton<PasswordHasher>();
        return services;
    }

    public static IServiceCollection ConfigureMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) => 
            {
                cfg.Host(CreateUserQueueConfig.Host, c => 
                {
                    c.Username(CreateUserQueueConfig.Username);
                    c.Password(CreateUserQueueConfig.Password);
                });
            });
        });
        return services;
    }

}