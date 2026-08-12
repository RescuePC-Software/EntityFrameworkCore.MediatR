using MediatR;

namespace RescuePC.Software.EntityFrameworkCore.MediatR.UnitTests.Fakes;

internal sealed record FakeStreamCommand : IStreamRequest<int>;
