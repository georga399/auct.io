using Microsoft.JSInterop.Infrastructure;
using Npgsql;

namespace Auctio.UserProfiles.Repositories;

public class UserProfileRepository
{
    private readonly NpgsqlDataSource _dataSource;
    public UserProfileRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<string> CreateUser(string id, string username, DateTime createdAt)
    {
        using (var connection = _dataSource.CreateConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO user_profiles (id, name, bio, createdAt)
                    VALUES (@id, @name, @bio, @createdAt)
                    RETURNING *;
                ";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", username);
                command.Parameters.AddWithValue("@bio", string.Empty);
                command.Parameters.AddWithValue("createdAt", createdAt);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    return id;
                }
            }
        }
    }

    public async Task<DTOs.UserProfile?> SetBio(string username, string bio)
    {
        using (var connection = _dataSource.CreateConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE user_profiles
                    SET bio = @bio
                    WHERE name = @name RETURNING name, bio;
                ";
                Console.WriteLine(username);
                command.Parameters.AddWithValue("@name", username);
                command.Parameters.AddWithValue("@bio", bio);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        Console.WriteLine(reader.GetString(0));
                        Console.WriteLine(reader.GetString(1));
                        return new DTOs.UserProfile(
                            reader.GetString(0),
                            reader.GetString(1),
                            DateTime.Now
                        );
                    }
                    else
                    {
                        Console.WriteLine("NO UPDATE");
                        return null;
                    }
                    
                }
            }
        }
    }

    public async Task<DTOs.UserProfile?> GetUser(string username)
    {
        using (var connection = _dataSource.CreateConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT * FROM user_profiles
                    WHERE name = @name;
                ";
                command.Parameters.AddWithValue("@name", username);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        return new DTOs.UserProfile(username, 
                            reader["bio"].ToString()!, 
                            Convert.ToDateTime(reader["createdAt"]));
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

    }

    public async Task<DTOs.UserProfile?> GetUserById(string id)
    {
        using (var connection = _dataSource.CreateConnection())
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT * FROM user_profiles
                    WHERE id = @id;
                ";
                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        return new DTOs.UserProfile(reader["name"].ToString()!, 
                            reader["bio"].ToString()!, 
                            Convert.ToDateTime(reader["createdAt"]));
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}