using Auctio.Items.Services;
using Npgsql;

namespace Auctio.Items;

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
                    CREATE TABLE IF NOT EXISTS items(
                        id SERIAL PRIMARY KEY NOT NULL,
                        name VARCHAR(100) NOT NULL,
                        userId TEXT NOT NULL,
                        userName TEXT NOT NULL,
                        createdAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        description TEXT,
                        cost FLOAT
                    );
                ";
                command.ExecuteNonQuery();
            }
        }
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<Repositories.ItemsRepository>();
        services.AddSingleton<ItemCacheService>();
        services.AddScoped<ItemService>();
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