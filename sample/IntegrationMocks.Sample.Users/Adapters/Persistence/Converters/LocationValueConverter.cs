using IntegrationMocks.Sample.Users.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace IntegrationMocks.Sample.Users.Adapters.Persistence.Converters;

internal sealed class LocationValueConverter : ValueConverter<Location, Guid>
{
    public LocationValueConverter()
        : base(model => model.Id, persistence => new Location(persistence))
    {
    }
}
