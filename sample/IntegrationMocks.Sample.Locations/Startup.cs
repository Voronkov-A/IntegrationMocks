using IntegrationMocks.Sample.Locations.Adapters.Persistence.Common;
using IntegrationMocks.Sample.Locations.Adapters.Persistence.Registration;
using IntegrationMocks.Sample.Locations.Adapters.WebApi.Registration;
using IntegrationMocks.Sample.Locations.Application.Registration;
using IntegrationMocks.Sample.Locations.Domain.Registration;

namespace IntegrationMocks.Sample.Locations;

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
        services.AddPersistenceMigrations();
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(x => x.MapControllers());
    }
}
