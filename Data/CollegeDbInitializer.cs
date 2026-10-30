using Microsoft.Data.SqlClient;

namespace SekaiADO24.Data;

public static class CollegeDbInitializer
{
    public static async Task EnsureDatabaseAsync(string connectionString, ILogger logger)
    {
        try
        {
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master",
                ConnectTimeout = 5
            };

            await using var connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = "IF DB_ID(N'CollegeDB') IS NULL CREATE DATABASE [CollegeDB];";
            await command.ExecuteNonQueryAsync();
            logger.LogInformation("База CollegeDB найдена или создана.");
        }
        catch (Exception exception)
        {
            logger.LogWarning("Не удалось создать CollegeDB автоматически: {Message}", exception.Message);
        }
    }
}
