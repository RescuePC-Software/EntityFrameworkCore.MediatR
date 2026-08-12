namespace RescuePC.Software.EntityFrameworkCore.MediatR.UnitTests;

internal sealed class CallTracker
{
    public bool WasCalled { get; private set; }

    public void Track() => WasCalled = true;
}
