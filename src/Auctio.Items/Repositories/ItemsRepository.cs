using Npgsql;

namespace Auctio.Items.Repositories;
public class ItemsRepository
{
    private readonly NpgsqlDataSource _dataSource;
    public ItemsRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    public async Task<DTOs.Item?> CreateItem(string userId, string userName, string name, string? description, double? cost)
    {
        try
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO items (name, userId, userName, description, cost) VALUES (@name, @userId, @userName, @description, @cost) RETURNING id, createdAt";
            command.Parameters.AddWithValue("name", name);
            command.Parameters.AddWithValue("userId", userId);
            command.Parameters.AddWithValue("userName", userName);
            command.Parameters.AddWithValue("description", description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("cost", cost.HasValue ? cost.Value : (object)DBNull.Value);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DTOs.Item(
                    reader.GetInt32(0),
                    name,
                    userId,
                    userName,
                    reader.GetDateTime(1),
                    description,
                    cost
                );
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while creating the item: {ex.Message}");
            return null;
        }
    }
    public async Task<DTOs.Item?> GetItem(int itemId)
    {
        try
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT id, name, userId, userName, createdAt, description, cost FROM items WHERE id = @id";
            command.Parameters.AddWithValue("id", itemId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DTOs.Item(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetDateTime(4),
                    reader.IsDBNull(5) ? null : reader.GetString(5),
                    reader.IsDBNull(6) ? null : (double?)reader.GetDouble(6)
                );
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while fetching the item: {ex.Message}");
            return null;
        }
    }
    public async Task<IEnumerable<DTOs.Item>> GetItems(string? username)
    {
        try
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var command = connection.CreateCommand();

            if (string.IsNullOrEmpty(username))
            {
                command.CommandText = "SELECT id, name, userId, userName, createdAt, description, cost FROM items ORDER BY createdAt DESC";
            }
            else
            {
                command.CommandText = "SELECT id, name, userId, userName, createdAt, description, cost FROM items WHERE userName = @username ORDER BY createdAt DESC";
                command.Parameters.AddWithValue("username", username);
            }

            using var reader = await command.ExecuteReaderAsync();
            var items = new List<DTOs.Item>();

            while (await reader.ReadAsync())
            {
                items.Add(new DTOs.Item(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetDateTime(4),
                    reader.IsDBNull(5) ? null : reader.GetString(5),
                    reader.IsDBNull(6) ? null : (double?)reader.GetDouble(6)
                ));
            }

            return items;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while fetching items: {ex.Message}");
            return Enumerable.Empty<DTOs.Item>();
        }
    }
    public async Task<bool> DeleteItem(int itemId)
    {
        try
        {
            using var connection = await _dataSource.OpenConnectionAsync();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM items WHERE id = @id";
            command.Parameters.AddWithValue("id", itemId);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while deleting the item: {ex.Message}");
            return false;
        }
    }
}