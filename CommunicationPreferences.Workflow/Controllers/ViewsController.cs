using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using Elsa;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreDemoApp.Controllers
{
    [Route("_views")]
    public class ViewsController : ControllerBase
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] {"value1", "value2"};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (id.EndsWith(".js"))
            {
                return Content(await typeof(ConfigController).GetTypeInfo().Assembly
                        .GetManifestResourceStream($"CommunicationPreferences.Workflow._views.{id}")
                        .ReadStringToEndAsync(), " application/javascript");

            }
            return Content(await typeof(ConfigController).GetTypeInfo().Assembly
                    .GetManifestResourceStream($"CommunicationPreferences.Workflow._views.{id}.html")
                    .ReadStringToEndAsync(), MediaTypeNames.Text.Html);


        }

        [HttpGet("js/{id}")]
        public async Task<IActionResult> GetJs(int id)
        {
            var file = $"CommunicationPreferences.Workflow._js.{id}";
            Console.WriteLine("reading " + $"{file}") ;
            return Content(await typeof(ConfigController).GetTypeInfo().Assembly
                .GetManifestResourceStream(file)
                .ReadStringToEndAsync(), " application/javascript");

        }
    }

    [Route("_js")]
    public class JsController : ControllerBase
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] {"value1", "value2"};
        }
 

        [HttpGet("_js/{id}")]
        public async Task<IActionResult> GetJs(int id)
        {
            var file = $"CommunicationPreferences.Workflow._js.{id}";
            Console.WriteLine("reading " + $"{file}") ;
            return Content(await typeof(ConfigController).GetTypeInfo().Assembly
                .GetManifestResourceStream(file)
                .ReadStringToEndAsync(), " application/javascript");

        }
    }
}