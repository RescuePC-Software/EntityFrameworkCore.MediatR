using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RescuePC.Software.EntityFrameworkCore.Behaviors;
using RescuePC.Software.EntityFrameworkCore.MediatR.UnitTests.Fakes;
using MediatR;

namespace RescuePC.Software.EntityFrameworkCore.MediatR.UnitTests;

public class ServiceCollectionExtensionsTests
{
    private static IServiceCollection BuildServices(IEnumerable<Type>? behaviors = null)
    {
        var services = new ServiceCollection();
        services.AddMediatR<FakeUnitOfWork>(
            [Assembly.GetExecutingAssembly()],
            behaviors ?? []
        );
        return services;
    }

    [Fact]
    public void AddMediatR_RegistersPipelineBehavior()
    {
        var services = BuildServices([typeof(FakePipelineBehavior<,>)]);

        var descriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IPipelineBehavior<,>) &&
            d.ImplementationType == typeof(FakePipelineBehavior<,>));

        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    [Fact]
    public void AddMediatR_RegistersMultiplePipelineBehaviors()
    {
        var services = BuildServices([typeof(FakePipelineBehavior<,>), typeof(AnotherFakePipelineBehavior<,>)]);

        var descriptors = services
            .Where(d => d.ServiceType == typeof(IPipelineBehavior<,>))
            .Select(d => d.ImplementationType)
            .ToList();

        Assert.Contains(typeof(FakePipelineBehavior<,>), descriptors);
        Assert.Contains(typeof(AnotherFakePipelineBehavior<,>), descriptors);
    }

    [Fact]
    public void AddMediatR_RegistersUnitOfWorkBehavior_ForCommandHandler()
    {
        var services = BuildServices();

        var serviceType = typeof(IPipelineBehavior<FakeCommand, Unit>);
        var expectedImpl = typeof(UnitOfWorkBehavior<FakeCommand, Unit, FakeUnitOfWork>);

        var descriptor = services.FirstOrDefault(d =>
            d.ServiceType == serviceType &&
            d.ImplementationType == expectedImpl);

        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    [Fact]
    public void AddMediatR_RegistersStreamUnitOfWorkBehavior_ForStreamCommandHandler()
    {
        var services = BuildServices();

        var serviceType = typeof(IStreamPipelineBehavior<FakeStreamCommand, int>);
        var expectedImpl = typeof(StreamUnitOfWorkBehavior<FakeStreamCommand, int, FakeUnitOfWork>);

        var descriptor = services.FirstOrDefault(d =>
            d.ServiceType == serviceType &&
            d.ImplementationType == expectedImpl);

        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    [Fact]
    public void AddMediatR_DoesNotRegisterUnitOfWorkBehavior_WhenRequestIsNotCommand()
    {
        var services = BuildServices();

        var serviceType = typeof(IPipelineBehavior<FakeQuery, Unit>);
        var unexpectedImpl = typeof(UnitOfWorkBehavior<FakeQuery, Unit, FakeUnitOfWork>);

        var descriptor = services.FirstOrDefault(d =>
            d.ServiceType == serviceType &&
            d.ImplementationType == unexpectedImpl);

        Assert.Null(descriptor);
    }
}
