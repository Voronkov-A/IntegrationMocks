using DotNet.Testcontainers.Builders;

namespace IntegrationMocks.Modules.Testcontainers;

public static class ContainerBuilderExtensions
{
    public static TBuilderEntity WithOutput<TBuilderEntity, TContainerEntity>(
        this TBuilderEntity builder,
        bool attachOutput = true)
        where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>
    {
        return attachOutput ? builder.WithOutputConsumer(new TestOutputConsumer()) : builder;
    }
}
