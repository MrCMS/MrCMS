namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Common
{
    public class SelectItemDto
    {
        public SelectItemDto(string id, string text)
        {
            Id = id;
            Text = text;
        }

        public string Id { get; set; }

        public string Text { get; set; }
    }
}