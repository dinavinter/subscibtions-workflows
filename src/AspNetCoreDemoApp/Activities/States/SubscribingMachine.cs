using System.Collections.Generic;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;

namespace AspNetCoreDemoApp.Activities
{
    public static class SubscribingMachineSchema
    {
        public const string State = "Subscribing";

        public static class Transitions
        {
            public const string EmailChannel = "Email-Channel";
            public const string PhoneChannel = "Phone-Channel";
        }
    }

    [Action(
        Category = "State Machine",
        DisplayName = SubscribingMachineSchema.State,
        Description = "Subscription Interaction",
        Outcomes = new[]
            { SubscribingMachineSchema.Transitions.EmailChannel, SubscribingMachineSchema.Transitions.PhoneChannel }
    )]
    public class SubscribingMachine : Activity
    {
        [ActivityInput(Hint = "The name of this state.",
            DefaultValue = SubscribingMachineSchema.State,
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string StateName { get; set; } = default!;

        [ActivityInput(
            Hint = "Enter one or more transition names.",
            UIHint = ActivityInputUIHints.Json,
            DefaultSyntax = SyntaxNames.Json,
            DefaultValue = new[]
            {
                SubscribingMachineSchema.Transitions.EmailChannel, SubscribingMachineSchema.Transitions.PhoneChannel
            },
            SupportedSyntaxes = new[] { SyntaxNames.Json },
            IsDesignerCritical = true
        )]
        public ISet<string> Transitions { get; set; } = new HashSet<string>();

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            context.SetVariable("CurrentState", StateName);
            context.JournalData.Add("Current State", StateName);
            return Outcomes(Transitions);
        }
    }
}