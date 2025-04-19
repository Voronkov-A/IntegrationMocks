using IntegrationMocks.Core;
using System.Text.Encodings.Web;

namespace IntegrationMocks.Modules.MySql;

public static class MySqlServiceExtensions
{
    public static string CreateMySqlConnectionString(
        this IInfrastructureService<MySqlServiceContract> service,
        string database = "mysql")
    {
        var username = UrlEncoder.Default.Encode(service.Contract.Username);
        var password = UrlEncoder.Default.Encode(service.Contract.Password);
        var host = service.Contract.Host;
        var port = service.Contract.Port;
        return $"server={host};uid={username};pwd={password};port={port}";
    }
}
