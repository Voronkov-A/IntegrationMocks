# IntegrationMocks

## Brief
Minimalistic set of simple helpers for writing integration and component tests. The key use cases are:
* Running infrastrcuture dependencies (like database, etc) in docker (or connecting to the ones already running).
* Mocking web API dependencies (i.e. service A interacts with service B - IntegrationMocks.Modules.AspNetCore helps to mock service B and use the mock in component tests of service A).
* Running the service under test for component tests with overridden configuration.

## Nuget
* [IntegrationMocks.Cleaner](https://www.nuget.org/packages/IntegrationMocks.Cleaner/)
* [IntegrationMocks.Core](https://www.nuget.org/packages/IntegrationMocks.Core/)
* [IntegrationMocks.Modules.AspNetCore](https://www.nuget.org/packages/IntegrationMocks.Modules.AspNetCore/)
* [IntegrationMocks.Modules.Postgres](https://www.nuget.org/packages/IntegrationMocks.Modules.Postgres/)
* [IntegrationMocks.Modules.Sql](https://www.nuget.org/packages/IntegrationMocks.Modules.Sql/)

## Components
* [IntegrationMocks.Cleaner](src/IntegrationMocks.Cleaner/README.md) - this tool may be used to clean up resources that were not released because of process termination.
* [IntegrationMocks.Core](src/IntegrationMocks.Core/README.md) - core library that contains abstractions and a minimal docker wrapper.
* [IntegrationMocks.Modules.AspNetCore](src/IntegrationMocks.Modules.AspNetCore/README.md) - library that contains basic wrappers for ASP.NET host services.
* [IntegrationMocks.Modules.Postgres](src/IntegrationMocks.Modules.Postgres/README.md) - library that contains wrappers for Postgres service.
* [IntegrationMocks.Modules.Sql](src/IntegrationMocks.Modules.Sql/README.md) - auxiliary library that contains common SQL abstractions.

## Samples
* [IntegrationMocks.Sample](sample/README.md) - sample.
* [IntegrationMocks.Modules.Postgres.Tests](test/IntegrationMocks.Modules.Postgres.Tests) - tests for Postgres.
* [IntegrationMocks.Modules.AspNetCore](test/IntegrationMocks.Modules.AspNetCore.Tests) - tests for API mocks and component SUTs.
