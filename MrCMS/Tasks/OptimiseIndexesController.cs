using Microsoft.AspNetCore.Mvc;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Tasks
{
    // TODO: optimise indexes
    public class OptimiseIndexesController : MrCMSUIController
    {
        public const string OptimiseIndexUrl = "optimise-index";
        //private readonly IIndexService _indexService;

        //public OptimiseIndexesController(IIndexService indexService)
        //{
        //    _indexService = indexService;
        //}

        [TaskExecutionKeyPasswordAuth]
        public ContentResult Execute(string type)
        {
            //_indexService.Optimise(type);
            return new ContentResult {Content = "Executed", ContentType = "text/plain"};
        }
    }
}