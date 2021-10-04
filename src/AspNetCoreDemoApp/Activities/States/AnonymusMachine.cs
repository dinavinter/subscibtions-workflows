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
    public static class AnonymousMachineSchema
    {
        public const string State = "anonymous";
        public static class Transitions
        {
            public const string Subscribing = SubscribingMachineSchema.State;
            public const string LoggingIn = LoggingInMachineSchema.State;

        }
    }

    [Action(
        Category = "State Machine",
        DisplayName = AnonymousMachineSchema.State,
        Description = "Puts the workflow into a specified auth state.",
        Outcomes = new[] { AnonymousMachineSchema.Transitions.Subscribing,  AnonymousMachineSchema.Transitions.LoggingIn }
    )]
    public class AnonymousMachine : Activity
    {
        [ActivityInput(Hint = "The name of this state.",
            DefaultValue = AnonymousMachineSchema.State,
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string StateName { get; set; } = default!;

        [ActivityInput(
            Hint = "Enter one or more transition names.",
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = SyntaxNames.Json,
            DefaultValue = new[] {  AnonymousMachineSchema.Transitions.Subscribing,  AnonymousMachineSchema.Transitions.LoggingIn },
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