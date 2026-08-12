using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RescuePC.Software.EntityFrameworkCore.MediatR.UnitTests.Fakes;
using MediatR;

namespace RescuePC.Software.EntityFrameworkCore.MediatR.UnitTests;

public class PipelineExecutionTests
{
    private static ServiceProvider BuildServiceProvider<TUnitOfWork>(
        IEnumerable<Type>? behaviors = null,
        Action<IServiceCollection>? configure = null)
        where TUnitOfWork : class, IUnitOfWork
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediatR<TUnitOfWork>(
            [Assembly.GetExecutingAssembly()],
            behaviors ?? []
        );
        configure?.Invoke(services);
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task SendCommand_PipelineBehaviorIsExecuted()
    {
        var tracker = new CallTracker();

        await using var provider = BuildServiceProvider<FakeUnitOfWork>(
            behaviors: [typeof(TrackingPipelineBehavior<,>)],
            configure: services =>
            {
                services.AddSingleton(tracker);
                services.AddSingleton<FakeUnitOfWork>();
            }
        );

        var mediator = provider.GetRequiredService<IMediator>();
        await mediator.Send(new FakeCommand());

        Assert.True(tracker.WasCalled);
    }

    [Fact]
    public async Task SendCommand_UnitOfWorkSaveChangesIsCalled()
    {
        var unitOfWork = new TrackingUnitOfWork();

        await using var provider = BuildServiceProvider<TrackingUnitOfWork>(
            configure: services =>
            {
                services.AddSingleton(unitOfWork);
                services.AddSingleton<IUnitOfWork>(unitOfWork);
            }
        );

        var mediator = provider.GetRequiredService<IMediator>();
        await mediator.Send(new FakeCommand());

        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task SendCommand_UnitOfWorkIsNotCalled_WhenRequestIsNotCommand()
    {
        var unitOfWork = new TrackingUnitOfWork();

        await using var provider = BuildServiceProvider<TrackingUnitOfWork>(
            configure: services =>
            {
                services.AddSingleton(unitOfWork);
                services.AddSingleton<IUnitOfWork>(unitOfWork);
            }
        );

        var mediator = provider.GetRequiredService<IMediator>();
        await mediator.Send(new FakeQuery());

        Assert.False(unitOfWork.SaveChangesCalled);
    }
}
