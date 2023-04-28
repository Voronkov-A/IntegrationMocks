namespace IntegrationMocks.Library.Sql;

public class SqlServiceContract
{
    public SqlServiceContract(string username, string password, string host, int port)
    {
        Username = username;
        Password = password;
        Host = host;
        Port = port;
    }

    public string Username { get; }

    public string Password { get; }

    public string Host { get; }

    public int Port { get; }
}
