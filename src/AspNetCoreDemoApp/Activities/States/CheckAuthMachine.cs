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
    public static class AuthMachineSchema
    {
        public const string State = "Authenticating";
        public static class Transitions
        {
            public const string LoggedIn = LoggedInMachineSchema.State;
            public const string Anonymous = AnonymousMachineSchema.State;

        }
    }

    [Action(
        Category = "State Machine",
        DisplayName = AuthMachineSchema.State,
        Description = "Puts the workflow into a specified auth state.",
        Outcomes = new[] { AuthMachineSchema.Transitions.LoggedIn,  AuthMachineSchema.Transitions.Anonymous }
    )]
    public class CheckAuthMachine : Activity
    {
        [ActivityInput(Hint = "The name of this state.",
            DefaultValue = AuthMachineSchema.State,
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string StateName { get; set; } = default!;

        [ActivityInput(
            Hint = "Enter one or more transition names.",
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = SyntaxNames.Json,
            DefaultValue = new[] {  AuthMachineSchema.Transitions.LoggedIn,  AuthMachineSchema.Transitions.Anonymous },
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