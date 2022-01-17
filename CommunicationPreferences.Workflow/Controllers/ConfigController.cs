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
    [Route("_api/[controller]")]
    public class ConfigController : ControllerBase
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
            return Content(await typeof(ConfigController).GetTypeInfo().Assembly
                    .GetManifestResourceStream($"CommunicationPreferences.Workflow._api.config.{id}.json")
                    .ReadStringToEndAsync(), MediaTypeNames.Application.Json) ;

        }
    }
}