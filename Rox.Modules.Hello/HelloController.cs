using Microsoft.AspNetCore.Mvc;
using System;

namespace Rox.Modules.Hello
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> GetHello()
        {
            return "hello world!";
        }
    }
}
