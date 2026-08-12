using MediatR;

namespace RescuePC.Software.EntityFrameworkCore.MediatR.UnitTests.Fakes;

internal sealed record FakeQuery : IRequest<Unit>;
