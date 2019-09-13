using System;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Grant
{
    public class PersistedGrantDto
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string ClientId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? Expiration { get; set; }
        public string Data { get; set; }
    }
}