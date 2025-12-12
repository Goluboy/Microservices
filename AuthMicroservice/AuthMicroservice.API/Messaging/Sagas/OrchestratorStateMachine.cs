using Core.Messaging.Contracts;
using MassTransit;

namespace AuthMicroservice.API.Messaging.Sagas;

public class OrchestratorStateMachine : MassTransitStateMachine<OrchestratorState>
{
    public State WaitingForRegistration { get; private set; }

    public Event<RegisterUserRequested> RegisterRequested { get; private set; }
    public Event<RegisterUserCompleted> RegisterCompleted { get; private set; }

    public OrchestratorStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => RegisterRequested, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => RegisterCompleted, x => x.CorrelateById(m => m.Message.CorrelationId));

        Initially(
            When(RegisterRequested)
                .Then(ctx =>
                {
                    ctx.Saga.Email = ctx.Message.Email;
                    ctx.Saga.Created = DateTime.UtcNow;
                })
                .TransitionTo(WaitingForRegistration)
        );

        During(WaitingForRegistration,
            When(RegisterCompleted)
                .Then(ctx =>
                {
                    ctx.Saga.UserId = ctx.Message.UserId;
                    // orchestrator could now instruct other services (e.g., profile create)
                })
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
}