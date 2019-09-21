using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.WebApi.Api;
using MrCMS.Web.Apps.WebApi.Api.Controllers;
using MrCMS.Website.Controllers;
using X.PagedList;

namespace MrCMS.Web.Apps.WebApi.Controllers
{


    [Route("api/v1/[controller]")]
    public class SampleController : MrCMSApiController
    {

       
        [Route("TodoItems")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<string>>> GetTodoItems()
        {
            var result  = new string[] { "Charles", "Ejedawe"};
            return CreatedAtAction("GetById", result);
            return await result.ToListAsync();
        }

        [HttpGet]
        public Task<IActionResult> GetById()
        {
            return Task.FromResult<IActionResult>(NoContent());
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
