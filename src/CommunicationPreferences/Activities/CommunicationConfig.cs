using System;
using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using Elsa;
using Elsa.Activities.Http;
using Elsa.Attributes;
using Elsa.Builders;
using Elsa.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using NetBox.Extensions;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CommunicationPreferences.Workflow
{
    public class CommunicationConfig : CompositeActivity
    {
        private IHttpContextAccessor _contextAccessor;

        public object Config = new
        {
            consentInstanceId = "consent1",
            consentRequired = true,
            promptUI = "consentPopup",
            postPromptUI = "repromptDialog",
            uiConfig = new
            {
                overlay = true
            },
            purposeConsentRequired = new[] {"purpose-analytics", "purpose-marketing"}
        };

        public CommunicationConfig(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public override void Build(ICompositeActivityBuilder builder)
        {
            builder.StartWith<HttpEndpoint>(activity => activity
                    .WithPath("/preferences/communication")
                    .WithMethod(HttpMethod.Post.ToString()))
                .WriteHttpResponse(x => x
                    .WithContent(ctx => JsonContent.Create(Config).JsonSerialise())
                    .WithContentType("application/json"));
                // .Redirect(new Uri("/_content/CommunicationPreferences/communication.json", UriKind.Relative));

            base.Build(builder);
        }
    }
}