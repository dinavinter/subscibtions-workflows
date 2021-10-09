using System;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Web;
using Elsa;
using Elsa.Activities.Http;
using Elsa.Activities.Http.Models;
using Elsa.Activities.JavaScript;
using Elsa.Activities.Primitives;
using Elsa.Builders;
using Elsa.Services.Models;
using NetBox.Extensions;

namespace CommunicationPreferences.Workflow
{
    public class CommunicationPreferencesWorkflow : IWorkflow
    {
        public void Build(IWorkflowBuilder builder)
        {
            builder.StartWith<CommunicationConfig>();
            //   builder.StartWith<HttpEndpoint>(activity => activity
            //             .WithPath("/preferences/communication")
            //             .WithMethod(HttpMethod.Get.ToString()))
            //             //.WithPolicy("public"))
            //         .WithName("GetSubscriptionConfig")
            //         .WithId("SubscriptionConfigRequest")
            //         .WithDisplayName("Get Subscription Config")
            //         .WithDescription("Request configuration for a given subscription collection")
            //         .SetVariable(x => x
            //             .WithVariableName("subscription")
            //             .WithValue((x) => new Uri(x.GetInput<HttpRequestModel>()!.Path).Segments.Last())
            //
            //         )
            //
            //         .RunJavaScript(x => x.WithScript(@"return {
            //      globalVendorListLocation: 'https://vendorlist.consensu.org/vendorlist.json',
            //      globalConsentLocation: './config/portal.html',
            //      customPurposeListLocation: './config/purposes.json',
            //      storeConsentGlobally: true,
            //      storePublisherData: true,
            //      logging: 'debug',
            //      allowedVendorIds: [1, 2, 3]
            // });"))
            //         .WithName("FetchConfig")
            //         .WithId("_fetchConfig")
            //
            //         .WriteHttpResponse(x => x
            //             .WithContent(ctx => ctx.Input)
            //             .WithContentType("application/json"));
            //
            //
            //
        }
    }
}