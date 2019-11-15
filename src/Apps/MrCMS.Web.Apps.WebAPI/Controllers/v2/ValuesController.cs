using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.WebApi.Api.Controllers;
using X.PagedList;

namespace MrCMS.Web.Apps.WebApi.Controllers.v2
{


    [ApiVersion("2")]
    public class SampleController : MrCMSApiController
    {

       
        [Route("TodoItems")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<string>>> GetTodoItems()
        {
            var result  = new string[] { "Charles 2.0 ", "Ejedawe 2.0"};
            return CreatedAtAction("GetById", result);
            return await result.ToListAsync();
        }

        [HttpGet]
        public Task<IActionResult> GetById()
        {
            return Task.FromResult<IActionResult>(NoContent());
        }

        [HttpGet]
        public String Get()
        {
            return "Version 2.0";
        }
    }


    public class SampleAuthController : MrCMSAuthApiController
    {


        [Route("TodoItems")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<string>>> GetTodoItems()
        {
            var result = new string[] { "Charles", "Ejedawe" };
            return CreatedAtAction("GetById", result);
            return await result.ToListAsync();
        }

        [HttpGet]
        public Task<IActionResult> GetById()
        {
            return Task.FromResult<IActionResult>(NoContent());
        }
    }
}
