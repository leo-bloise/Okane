using Npgsql;
using Okane.User.Application.Interfaces;

namespace Okane.Api.Infrastructure.Persistence;

public sealed class UserRepository(NpgsqlConnectionFactory connectionFactory) : IUserRepository
{
    public async Task<User.Domain.User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, name, email, password_hash, created_at FROM users WHERE id = @id";
        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<User.Domain.User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, name, email, password_hash, created_at FROM users WHERE email = @email";
        command.Parameters.AddWithValue("email", email.Trim().ToLowerInvariant());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task AddAsync(User.Domain.User user, CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO users (id, name, email, password_hash, created_at)
            VALUES (@id, @name, @email, @passwordHash, @createdAt)
            """;
        command.Parameters.AddWithValue("id", user.Id);
        command.Parameters.AddWithValue("name", user.Name);
        command.Parameters.AddWithValue("email", user.Email);
        command.Parameters.AddWithValue("passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("createdAt", user.CreatedAt);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task UpdateAsync(User.Domain.User user, CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE users
            SET name = @name, email = @email, password_hash = @passwordHash
            WHERE id = @id
            """;
        command.Parameters.AddWithValue("id", user.Id);
        command.Parameters.AddWithValue("name", user.Name);
        command.Parameters.AddWithValue("email", user.Email);
        command.Parameters.AddWithValue("passwordHash", user.PasswordHash);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static User.Domain.User Map(NpgsqlDataReader reader) => User.Domain.User.FromPersistence(
        reader.GetGuid(0),
        reader.GetString(1),
        reader.GetString(2),
        reader.GetString(3),
        reader.GetFieldValue<DateTimeOffset>(4));
}
