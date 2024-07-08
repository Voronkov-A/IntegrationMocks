using IntegrationMocks.Core;
using IntegrationMocks.Modules.Sql;
using System.Text.Encodings.Web;

namespace IntegrationMocks.Modules.Yugabyte;

public static class SqlServiceExtensions
{
    public static string CreateYugabyteConnectionString(
        this IInfrastructureService<SqlServiceContract> service,
        string database = "yugabyte")
    {
        var username = UrlEncoder.Default.Encode(service.Contract.Username);
        var password = UrlEncoder.Default.Encode(service.Contract.Password);
        var host = service.Contract.Host;
        var port = service.Contract.Port;
        return
            $"User ID={username};Password={password};Server={host};Port={port};Database={database};Pooling=true;MinPoolSize=1;MaxPoolSize=30;Write Buffer Size=16384;Enlist=true;";
    }
}
