using Core.Messaging.Contracts;
using MassTransit;

namespace AuthMicroservice.API.Messaging.Sagas;
public class CoordinatorStateMachine : MassTransitStateMachine<CoordinatorState>
{
    public State? LoggedIn { get; private set; }

    public Event<LoginRequested>? LoginRequested { get; private set; }

    public CoordinatorStateMachine()
    {
        InstanceState(x => x.CurrentState);
        
        Event(() => LoginRequested, x =>
            x.CorrelateById(context => context.Message.CorrelationId)
             .SelectId(context => context.Message.CorrelationId == Guid.Empty ? Guid.NewGuid() : context.Message.CorrelationId));
        
        DuringAny(
            When(LoginRequested)
                .Then(context =>
                {
                    context.Saga.Email = context.Message.Email;
                    context.Saga.Created = DateTime.UtcNow;
                })
                .ThenAsync(async ctx =>
                {
                    // in a real app you'd call auth service; here we simulate token creation
                    var token = Guid.NewGuid().ToString();
                    ctx.Saga.Token = token;

                    await ctx.Publish(new LoginCompleted
                    {
                        CorrelationId = ctx.Saga.CorrelationId,
                        Email = ctx.Saga.Email!,
                        Token = token
                    });
                })
                .TransitionTo(LoggedIn)
        );

        SetCompletedWhenFinalized();
    }
}