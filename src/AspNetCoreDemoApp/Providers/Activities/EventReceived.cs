using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa;
using Elsa.Activities.Conductor.Models;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Metadata;
using Elsa.Services;
using Elsa.Services.Models;

// ReSharper disable once CheckNamespace
namespace AspNetCoreDemoApp.Providers.Activities
{
    [Trigger(
        Category = "state.Events",
        Description = "Waits for an event sent from your application."
        // Outcomes = typeof(EventReceived)

    )]
    public class EventReceived : Activity, IOutcomesProvider
    {
        [ActivityInput(
            Label = "Event",
            Hint = "The event to wait for.",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid },
            IsDesignerCritical = true,
            Options = "DisplayName"
        )]
        public string EventName { get; set; } = default!;

        [ActivityInput(
            Hint = "The name of this state.",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid },
            DefaultSyntax = SyntaxNames.JavaScript,
            DefaultValue = "`${context.CurrentState}.${context.activity.EventName}`"

            )]
        public string StateName { get; set; } = default!;


        [ActivityInput(
            Hint = "Enter one or more possible outcomes for this event.",
            UIHint = "outcomes",
            DefaultSyntax = SyntaxNames.Json,
            SupportedSyntaxes = new[] { SyntaxNames.Json },
             IsDesignerCritical = true
        )]
        public ISet<string> OutcomeNames { get; set; } = new HashSet<string>();

        [ActivityOutput(Hint = "Any input that was sent along with the event from your application.")]
        public object? Payload { get; set; }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context) =>
            context.IsFirstPass ? OnExecuteInternal(context) : Suspend();

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context) =>
            OnExecuteInternal(context);

        private IActivityExecutionResult OnExecuteInternal(ActivityExecutionContext context)
        {
            var eventModel = context.GetInput<EventModel>()!;
            var outcomes = eventModel.Outcomes;

            if (outcomes?.Any() == false)
                outcomes = new[] { "Then" };

            Payload = eventModel.Payload;
            context.JournalData.Add("Payload", eventModel.Payload);
            context.SetVariable("CurrentState", StateName);
            context.JournalData.Add("Current State", StateName);

            return base.Outcomes(OutcomeNames!);
        }

        public ValueTask<IEnumerable<string>> GetOutcomesAsync(CancellationToken cancellationToken)
        {
            return new ( OutcomeNames);
        }
    }
}