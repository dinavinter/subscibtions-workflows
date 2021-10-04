using System.Threading.Tasks;
using Elsa;
using Elsa.Activities.Conductor.Models;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;

namespace AspNetCoreDemoApp.Providers.Activities
{
    [Action(
        Category = "state.Actions",
        DisplayName = "Custom Action",
        Description = "Executes action- doesn't effect state.",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class ExecuteAction : Activity
    {
        private readonly IEventPublisher _eventPublisher;

        public ExecuteAction(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        [ActivityInput(
            Label = "Command",
            Hint = "The command to send.",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid }
        )]
        public string CommandName { get; set; } = default!;

        [ActivityInput(
            UIHint = ActivityInputUIHints.MultiLine,
            Hint = "Optional data to send to your application.",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid, SyntaxNames.Json })]
        public object? Payload { get; set; } = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            await _eventPublisher.PublishAsync(new SendCommandModel(CommandName, Payload, context.WorkflowInstance.Id));
            return Done();
        }
    }
}