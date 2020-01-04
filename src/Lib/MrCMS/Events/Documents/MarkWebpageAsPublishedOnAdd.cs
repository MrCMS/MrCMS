using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Events.Documents
{
    public class MarkWebpageAsPublishedOnAdd : OnDataAdding<Webpage> 
    {
        private readonly ICheckPublishedStatus _checkStatus;

        public MarkWebpageAsPublishedOnAdd(ICheckPublishedStatus checkStatus)
        {
            _checkStatus = checkStatus;
        }

        public override async Task<IResult> OnAdding(Webpage entity, DbContext context)
        {
            await _checkStatus.Check(entity);
            return await Success;
        }
    }
}