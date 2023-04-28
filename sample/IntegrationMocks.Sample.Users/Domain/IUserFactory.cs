namespace IntegrationMocks.Sample.Users.Domain;

public interface IUserFactory
{
    User CreateUser(string name, Location location);
}
