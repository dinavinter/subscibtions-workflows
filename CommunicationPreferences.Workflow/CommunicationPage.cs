using System;
using System.IO;
using System.Net;
using System.Reflection;
using Elsa;
using Elsa.Activities.Http;
using Elsa.Builders;
using Elsa.Services;
using NetBox.Extensions;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CommunicationPreferences.Workflow
{
    public class   @{
    }Page: CompositeActivity
    {
        private const string EmbeddedFile = "CommunicationPreferences.Workflow._views.index.html";
        public Func<Stream> IndexStream { get; set; } = () => typeof(CommunicationPage).GetTypeInfo().Assembly
            .GetManifestResourceStream(EmbeddedFile);


        public override void Build(ICompositeActivityBuilder builder)
        {
            builder
                .WriteHttpResponse(x => x.WithContent(async ()=>await IndexStream().ReadStringToEndAsync())
                .WithContentType("text/html;charset=utf-8")
                .WithStatusCode(HttpStatusCode.OK)
                );
            base.Build(builder);
        }
    }
}