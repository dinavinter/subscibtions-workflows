using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreDemoApp.Providers.Models;
using Elsa;
using Elsa.Activities.ControlFlow;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Builders;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Metadata;
using Elsa.Services;
using Elsa.Services.Models;
using EventModel = Elsa.Activities.Conductor.Models.EventModel;

// ReSharper disable once CheckNamespace
namespace AspNetCoreDemoApp.Providers.Activities
{
    [Trigger(
        Category = "state.Events",
        Description = "Waits for an event sent from your application.",
        Type = "EventReceived"
        // Outcomes = typeof(EventReceived)

    )]
    public class EventReceived :  Activity, IOutcomesProvider
    {
        [ActivityInput(
            Label = "Event",
            Hint = "The event to wait for.",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid },
            IsDesignerCritical = true,
            Category = "Common",
            Options = "DisplayName"
        )]
        public string EventName { get; set; } = default!;

        [ActivityInput(
            Hint = "The name of this state.",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid },
            DefaultSyntax = SyntaxNames.JavaScript,
            DefaultValue = "`${context.CurrentState}.${context.activity.EventName}`",
            IsReadOnly = true

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
        // [ActivityInput(Hint = "The conditions to evaluate.", UIHint = "switch-case-builder", DefaultSyntax = "Switch", IsDesignerCritical = true)]
        public ICollection<TransitionModel> Transitions { get; set; } = new List<TransitionModel>();
        public bool EnteredScope
        {
            get => GetState<bool>();
            set => SetState(value);
        }
        public OutcomeResult Outcome
        {
            get => GetState<OutcomeResult>();
            set => SetState(value);
        }

        // public override void Build(ICompositeActivityBuilder builder)
        // {
        //     builder.Switch(e =>
        //     {
        //         Transitions.Select(t => e.Add(t.Name, (ctx) =>
        //         {
        //             ctx.GetActivityPropertyAsync()
        //         }, outcome => outcome.Then(builder, activityBuilder => activityBuilder.Finish(
        //             t.Name)));
        //     })
        //     base.Build(builder);
        // }

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