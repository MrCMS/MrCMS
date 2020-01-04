using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Events.Documents
{
    public class MarkWebpageAsUnpublished : OnDataUpdating<Webpage>
    {
        private readonly IGetDateTimeNow _getDateTimeNow;

        public MarkWebpageAsUnpublished(IGetDateTimeNow getDateTimeNow)
        {
            _getDateTimeNow = getDateTimeNow;
        }

        public override Task<IResult> OnUpdating(Webpage entity, DbContext context)
        {
            var now = _getDateTimeNow.UtcNow;
            if (entity.Published && (entity.PublishOn == null || entity.PublishOn.Value.ToUniversalTime() > now))
            {
                entity.Published = false;
            }

            return Success;
        }
    }
}