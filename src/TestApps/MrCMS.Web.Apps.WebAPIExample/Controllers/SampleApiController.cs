using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.WebApi.Api.Controllers;
using X.PagedList;

namespace MrCMS.Web.Apps.WebAPIExample.Controllers
{


    
    public class SampleController : MrCMSApiController
    {

       
        [Route("TodoItems")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<string>>> GetTodoItems()
        {
            var result  = new string[] { "MrCMS", "Web API"};
            return CreatedAtAction("GetById", result);
           // return await result.ToListAsync();
        }

        [HttpGet]
        [Route("GetById")]
        public Task<IActionResult> GetById()
        {
            return Task.FromResult<IActionResult>(NoContent());
        }

        [HttpGet]
        public String Get()
        {
            return "Version 1.0";
        }
    }


    public class SampleAuthController : MrCMSAuthApiController
    {


        [Route("TodoItems")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<string>>> GetTodoItems()
        {
            var result = new string[] { "Authenticated MrCMS", "Web API" };
            return CreatedAtAction("GetById", result);
            return await result.ToListAsync();
        }

        [Route("GetById")]
        [HttpGet]
        public Task<IActionResult> GetById()
        {
            return Task.FromResult<IActionResult>(NoContent());
        }

        [HttpGet]
        public String Get()
        {
            return "Authenticated Version 1.0";
        }
    }


    public class SampleAdminController : MrCMSAdminApiController
    {


        [Route("TodoItems")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<string>>> GetTodoItems()
        {
            var result = new string[] { "Authenticated Admin MrCMS", "Web API" };
            return CreatedAtAction("GetById", result);
            return await result.ToListAsync();
        }


        [Route("GetById")]
        [HttpGet]
        public Task<IActionResult> GetById()
        {
            return Task.FromResult<IActionResult>(NoContent());
        }

        [HttpGet]
        public String Get()
        {
            return "Authenticated Version 1.0";
        }
    }
}
