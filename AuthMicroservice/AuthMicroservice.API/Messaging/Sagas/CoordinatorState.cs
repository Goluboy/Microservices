using MassTransit;

namespace AuthMicroservice.API.Messaging.Sagas;

public class CoordinatorState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public string? Email { get; set; }
    public Guid? UserId { get; set; }
    public string? Token { get; set; }
    public DateTime? Created { get; set; }
}