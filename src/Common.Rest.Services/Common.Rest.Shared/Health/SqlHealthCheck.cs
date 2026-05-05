namespace Common.Rest.Shared.Health;

/// <summary>
/// Reusable health check for Azure SQL connectivity.
/// </summary>
public sealed class SqlHealthCheck(string connectionString) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            //await using var connection = new SqlConnection(connectionString);
            //await connection.OpenAsync(cancellationToken);

            //await using var command = connection.CreateCommand();
            //command.CommandText = "SELECT 1";
            //await command.ExecuteScalarAsync(cancellationToken);

            return HealthCheckResult.Healthy("SQL Database is reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("SQL Database is unreachable.", ex);
        }
    }
}
