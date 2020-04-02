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

        public override async Task<IResult> OnUpdating(Webpage entity, DbContext context)
        {
            var now = await _getDateTimeNow.GetLocalNow();
            if (entity.Published && (entity.PublishOn == null || entity.PublishOn.Value > now))
            {
                entity.Published = false;
            }

            return await Success;
        }
    }
}