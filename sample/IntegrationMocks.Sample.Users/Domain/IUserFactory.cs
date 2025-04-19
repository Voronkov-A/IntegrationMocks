namespace IntegrationMocks.Sample.Users.Domain;

internal interface IUserFactory
{
    User CreateUser(string name, Location location);
}
