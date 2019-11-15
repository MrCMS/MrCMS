using System.Collections.Generic;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Grant
{
    public class PersistedGrantsDto
    {
        public PersistedGrantsDto()
        {
            PersistedGrants = new List<PersistedGrantDto>();
        }

        public string SubjectId { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public List<PersistedGrantDto> PersistedGrants { get; set; }
    }
}