using MediatR;

namespace RescuePC.Software.EntityFrameworkCore.MediatR.UnitTests.Fakes;

internal sealed class FakeCommandHandler : IRequestHandler<FakeCommand, Unit>
{
    public Task<Unit> Handle(FakeCommand request, CancellationToken cancellationToken) => Task.FromResult(Unit.Value);
}
