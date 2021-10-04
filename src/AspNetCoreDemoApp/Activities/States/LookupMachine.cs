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
    public static class LookupMachineSchema
    {
        public const string State = "lookup";

        public static class Transitions
        {
            public const string ChannelExists = "Channel-Exists";
            public const string ChannelNotExists = "Channel-Not-Exists";
        }
    }

    [Action(
        Category = "State Machine",
        DisplayName = LookupMachineSchema.State,
        Description = "Puts the workflow into a specified auth state.",
        Outcomes = new[]
            { AnonymousMachineSchema.Transitions.Subscribing, AnonymousMachineSchema.Transitions.LoggingIn }
    )]
    public class LookupMachine : Activity
    {
        [ActivityInput(Hint = "The name of this state.",
            DefaultValue = LookupMachineSchema.State,
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string StateName { get; set; } = default!;

        [ActivityInput(
            Hint = "Enter one or more transition names.",
            UIHint = ActivityInputUIHints.MultiText,
            DefaultSyntax = SyntaxNames.Json,
            DefaultValue = new[]
                { LookupMachineSchema.Transitions.ChannelExists, LookupMachineSchema.Transitions.ChannelNotExists },
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