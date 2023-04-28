using System.Text.Encodings.Web;
using IntegrationMocks.Core;

namespace IntegrationMocks.Library.Sql;

public static class SqlServiceExtensions
{
    public static string CreatePostgresConnectionString(
        this IInfrastructureService<SqlServiceContract> service,
        string database = "postgres")
    {
        var username = UrlEncoder.Default.Encode(service.Contract.Username);
        var password = UrlEncoder.Default.Encode(service.Contract.Password);
        var host = service.Contract.Host;
        var port = service.Contract.Port;
        return
            $"User ID={username};Password={password};Server={host};Port={port};Database={database};Pooling=true;MinPoolSize=1;MaxPoolSize=30;Write Buffer Size=16384;Enlist=true;";
    }
}
