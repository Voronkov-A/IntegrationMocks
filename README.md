# IntegrationMocks

## Brief
Minimalistic set of simple helpers for writing integration and component tests. The key use cases are:
* Running infrastrcuture dependencies (like database, etc) in docker (or connecting to the ones already running).
* Mocking web API dependencies (i.e. service A interacts with service B - IntegrationMocks.Web helps to mock service B and use the mock in component tests of service A).
* Running the service under test for component tests with overridden configuration.

## Nuget
* [IntegrationMocks.Cleaner](https://www.nuget.org/packages/IntegrationMocks.Cleaner/)
* [IntegrationMocks.Core](https://www.nuget.org/packages/IntegrationMocks.Core/)
* [IntegrationMocks.Library](https://www.nuget.org/packages/IntegrationMocks.Library/)
* [IntegrationMocks.Web](https://www.nuget.org/packages/IntegrationMocks.Web/)

## Components
* [IntegrationMocks.Cleaner](src/IntegrationMocks.Cleaner/README.md) - this tool may be used to clean up resources that were not released because of process termination.
* [IntegrationMocks.Core](src/IntegrationMocks.Core/README.md) - core library that contains abstractions and a minimal docker wrapper.
* [IntegrationMocks.Library](src/IntegrationMocks.Library/README.md) - library that contains implementations of typical infrastructure services.
* [IntegrationMocks.Web](src/IntegrationMocks.Web/README.md) - library that contains basic wrappers for ASP.NET host services.

## Samples
* [IntegrationMocks.Sample](sample/README.md) - sample.
* [IntegrationMocks.Library.Tests](test/IntegrationMocks.Library.Tests) - tests for infrastrcuture dependencies.
* [IntegrationMocks.Library.Web](test/IntegrationMocks.Web.Tests) - tests for API mocks and component SUTs.
