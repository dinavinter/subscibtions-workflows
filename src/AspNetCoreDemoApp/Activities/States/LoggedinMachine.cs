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
    public static class LoggedInMachineSchema
    {
        public const string State = "logged-in";
        public static class Transitions
        {
            public const string Subscribing = "subscribing";
            public const string LoggingOut = "anonymus";

        }
    }

    [Action(
        Category = "State Machine",
        DisplayName = LoggedInMachineSchema.State,
        Description = "Puts the workflow into a specified auth state.",
        Outcomes = new[] { LoggedInMachineSchema.Transitions.Subscribing,  LoggedInMachineSchema.Transitions.LoggingOut }
    )]
    public class LoggedInMachine : Activity
    {
        [ActivityInput(Hint = "The name of this state.",
            DefaultValue = LoggedInMachineSchema.State,
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string StateName { get; set; } = default!;

        [ActivityInput(
            Hint = "Enter one or more transition names.",
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = SyntaxNames.Json,
            DefaultValue = new[] {  LoggedInMachineSchema.Transitions.Subscribing,  LoggedInMachineSchema.Transitions.LoggingOut },
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