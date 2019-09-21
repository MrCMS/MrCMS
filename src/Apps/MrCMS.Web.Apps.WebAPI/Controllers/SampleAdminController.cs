using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.WebApi.Api.Controllers;
using X.PagedList;

namespace MrCMS.Web.Apps.WebApi.Controllers
{
    public class SampleAdminController : MrCMSAdminApiController
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