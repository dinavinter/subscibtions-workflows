using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AspNetCoreDemoApp.Controllers
{
    [Route("app/[controller]")]

    public class CommunicationController:Controller
    {

        [HttpGet ]
        public IActionResult Get()
        {
            return View("communication");
        }
    }
}