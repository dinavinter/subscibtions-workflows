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
    Category = "Conductor",
    DisplayName = "Update Account Attributes",
    Description = "Sends an account update command.", Outcomes = new string[] { "Done" })]
public class UpdateAccountAttributes : Activity
{
    [ActivityInput(Hint = "Account UID.",
        DefaultValue = "`${input.UID}`",
        SupportedSyntaxes = new[] { SyntaxNames.JavaScript },
        DefaultSyntax = SyntaxNames.JavaScript)]
    public object UID { get; set; } = default!;

    [ActivityInput(Hint = "Account Consents.",
        DefaultValue = "`${input.consents}`",
        SupportedSyntaxes = new[] { SyntaxNames.JavaScript },
        DefaultSyntax = SyntaxNames.JavaScript)]
    public object Consents { get; set; } = default!;

    [ActivityInput(Hint = "Account Profile.",
        DefaultValue = "`${input.profile}`",
        SupportedSyntaxes = new[] { SyntaxNames.JavaScript },
        DefaultSyntax = SyntaxNames.JavaScript)]
    public object Profile { get; set; } = default!;

    [ActivityInput(Hint = "Account Subscriptions.",
        DefaultValue = "`${input.subscriptions}`",
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
        context.JournalData.Add("Account Updated", payload);
        return Done(payload);
    }


}