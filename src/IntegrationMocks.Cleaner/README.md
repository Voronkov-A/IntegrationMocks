# IntegrationMocks.Cleaner

## Brief
This tool may be used to clean up resources that were not released because of process termination.  

## TCP/IP ports
`PortManager` locks host TCP/IP ports in order to start web servers or expose docker containers. Since tests may run in parallel processes it requires some synchronization. The default implementation (`PortManager.Default`) creates a file for every locked port in directory `Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $"{nameof(IntegrationMocks)}_{nameof(PortManager)}")`. The file will be deleted once the `IPort` instance is disposed or finalized. But in case of process termination the file may stay. So the IntegrationMocks.Cleaner just deletes all the files in the directory.

## Docker containers

`DefaultContainerNameRepositoryDirectoryPath` creates docker containers using `Ductus.FluentDocker` library. The default implementation (`DefaultContainerNameRepositoryDirectoryPath.Default`) also creates a file for every started container (the file name equals to the container name) in directory `Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $"{nameof(IntegrationMocks)}_{nameof(FluentDockerContainerManager)}")`. The container and the file will be deleted once the `IDockerContainer` is disposed or finalized. But in case of process termination the container or the file may stay. So the IntegrationMocks.Cleaner lists all the files in the directory and deletes them together with the containers.
