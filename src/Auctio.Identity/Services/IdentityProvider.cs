using Auctio.Shared.Jwt;
using Auctio.Shared.Masstransit;
using Npgsql;

namespace Auctio.Identity.Services;

public class IdentityProvider
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly PasswordHasher _passwordHasher;
    public IdentityProvider(NpgsqlDataSource dataSource, PasswordHasher passwordHasher)
    {
        _dataSource = dataSource;
        _passwordHasher = passwordHasher;
    }
    public async Task<string?> Login(string username, string password)
    {
        using var connection = await _dataSource.OpenConnectionAsync();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, passwordHash FROM users WHERE name = @username";
        command.Parameters.AddWithValue("username", username);
        using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }
        var userId = reader.GetString(0);
        var storedHash = reader.GetString(1);
        if (!_passwordHasher.Verify(storedHash, password))
        {
            return null;
        }
        return GenerateJwt.Generate(userId, username);
    }
    public async Task<CreateUser?> Register(string username, string password)
    {
        using var connection = await _dataSource.OpenConnectionAsync();

        // Check if username already exists
        using (var checkCommand = connection.CreateCommand())
        {
            checkCommand.CommandText = "SELECT COUNT(*) FROM users WHERE name = @username";
            checkCommand.Parameters.AddWithValue("username", username);
            var count = (long)(await checkCommand.ExecuteScalarAsync())!;
            if (count > 0)
            {
                return null;
            }
        }

        // Insert new user
        using (var insertCommand = connection.CreateCommand())
        {
            var hashedPassword = _passwordHasher.Hash(password);
            insertCommand.CommandText = "INSERT INTO users (name, passwordHash) VALUES (@username, @passwordHash) RETURNING id, name, createdAt";
            insertCommand.Parameters.AddWithValue("username", username);
            insertCommand.Parameters.AddWithValue("passwordHash", hashedPassword);

            using var reader = await insertCommand.ExecuteReaderAsync();
            await reader.ReadAsync();
            return new CreateUser(reader.GetString(0),reader.GetString(1), reader.GetDateTime(2));
        }
        
    }

}