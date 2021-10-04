using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Elsa.Activities.Conductor;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;


[Action(
    Category = "state.Actions",
    DisplayName = "Update Downstream System",
    Description = "Sends updates to downstream system.", Outcomes = new string[] { "Done" })]
public class UpdateDownStreamSystem : Activity
{
    [ActivityInput(Hint = "Account UID.",
        DefaultValue = "`${context.identity.UID}`",
        SupportedSyntaxes = new[] { SyntaxNames.JavaScript },
        DefaultSyntax = SyntaxNames.JavaScript)]
    public object UID { get; set; } = default!;

    [ActivityInput(Hint = "Account Consents.",
        DefaultValue = "`${merge(context.account?.consent, context.attributes.consents}`",
        SupportedSyntaxes = new[] { SyntaxNames.JavaScript },
        DefaultSyntax = SyntaxNames.JavaScript)]
    public object Consents { get; set; } = default!;

    [ActivityInput(Hint = "Account Profile.",
        DefaultValue = "`${merge(context.account?.profile, context.attributes.profile}`",
        SupportedSyntaxes = new[] { SyntaxNames.JavaScript },
        DefaultSyntax = SyntaxNames.JavaScript)]
    public object Profile { get; set; } = default!;

    [ActivityInput(Hint = "Account Subscriptions.",
        DefaultValue =  "`${merge(context.account?.subscriptions, context.attributes.subscriptions}`",
        SupportedSyntaxes = new[] { SyntaxNames.JavaScript },
        DefaultSyntax = SyntaxNames.JavaScript)]
    public object Subscriptions { get; set; } = default!;

    [ActivityInput(Hint = "Account PhoneNumber.",
        DefaultValue = "`${input.channel.phone}`",
        SupportedSyntaxes = new[] { SyntaxNames.JavaScript },
        DefaultSyntax = SyntaxNames.JavaScript)]
    public object PhoneNumber { get; set; } = default!;

    [ActivityInput(Hint = "Account Email.",
        DefaultValue = "`${input.channel.email}`",
        SupportedSyntaxes = new[] { SyntaxNames.JavaScript },
        DefaultSyntax = SyntaxNames.JavaScript)]
    public object Email { get; set; } = default!;


    protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
    {
        var payload = new
        {
            UID,
            Consents,
            Profile,
            Subscriptions,
            PhoneNumber,
            Email
        };
        context.SetContainerState("Payload", payload);
        context.JournalData.Add("DownStream Updated", payload);
        return Done(payload);
    }


}