using MediatR;

namespace RescuePC.Software.EntityFrameworkCore.MediatR.UnitTests.Fakes;

internal sealed class FakeQueryCommandHandler : IRequestHandler<FakeQuery, Unit>
{
    public Task<Unit> Handle(FakeQuery request, CancellationToken cancellationToken) => Task.FromResult(Unit.Value);
}
