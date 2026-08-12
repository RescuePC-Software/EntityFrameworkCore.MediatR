namespace RescuePC.Software.EntityFrameworkCore.MediatR.UnitTests.Fakes;

internal sealed class TrackingUnitOfWork : IUnitOfWork
{
    public bool SaveChangesCalled { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCalled = true;
        return Task.FromResult(0);
    }
}
