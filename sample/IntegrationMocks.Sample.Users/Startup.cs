using IntegrationMocks.Sample.Users.Adapters.Locations;
using IntegrationMocks.Sample.Users.Adapters.Locations.Registration;
using IntegrationMocks.Sample.Users.Adapters.Persistence.Common;
using IntegrationMocks.Sample.Users.Adapters.Persistence.Registration;
using IntegrationMocks.Sample.Users.Adapters.WebApi.Registration;
using IntegrationMocks.Sample.Users.Application.Registration;
using IntegrationMocks.Sample.Users.Domain.Registration;

namespace IntegrationMocks.Sample.Users;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddDomain();
        services.AddApplication();
        services.AddWebApi();
        services.AddPersistence(
            _configuration.GetSection("persistence").Get<PersistenceOptions>()
            ?? throw new SystemException("Persistence section is required."));
        services.AddLocations(
            _configuration.GetSection("locations").Get<LocationsOptions>()
            ?? throw new SystemException("Locations section is required."));
        services.AddPersistenceMigrations();
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(x => x.MapControllers());
    }
}
