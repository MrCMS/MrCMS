using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Configuration;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Configuration
{
    public class ApiResourcesDto
	{
		public ApiResourcesDto()
		{
			ApiResources = new List<ApiResourceDto>();
		}

		public int PageSize { get; set; }

		public int TotalCount { get; set; }

		public List<ApiResourceDto> ApiResources { get; set; }
	}
}