using System.Collections.Generic;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Configuration
{
	public class ClientsDto
	{
		public ClientsDto()
		{
			Clients = new List<ClientDto>();
		}

		public List<ClientDto> Clients { get; set; }

		public int TotalCount { get; set; }		

		public int PageSize { get; set; }
	}
}
