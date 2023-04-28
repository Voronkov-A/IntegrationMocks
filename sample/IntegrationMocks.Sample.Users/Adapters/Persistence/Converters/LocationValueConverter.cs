using IntegrationMocks.Sample.Users.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IntegrationMocks.Sample.Users.Adapters.Persistence.Converters;

public class LocationValueConverter : ValueConverter<Location, Guid>
{
    public LocationValueConverter() : base(model => model.Id, persistence => new Location(persistence))
    {
    }
}
