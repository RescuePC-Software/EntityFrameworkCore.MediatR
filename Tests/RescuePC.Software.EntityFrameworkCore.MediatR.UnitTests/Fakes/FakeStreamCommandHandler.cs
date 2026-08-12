using MediatR;

namespace RescuePC.Software.EntityFrameworkCore.MediatR.UnitTests.Fakes;

internal sealed class FakeStreamCommandHandler : IStreamRequestHandler<FakeStreamCommand, int>
{
    public IAsyncEnumerable<int> Handle(FakeStreamCommand request, CancellationToken cancellationToken)
        => AsyncEnumerable.Empty<int>();
}
