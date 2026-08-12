namespace RescuePC.Software.EntityFrameworkCore.MediatR.UnitTests.Fakes;

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);
}
