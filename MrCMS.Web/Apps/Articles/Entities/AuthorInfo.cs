using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Articles.Entities
{
    public class AuthorInfo : UserProfileData
    {
        public virtual string Bio { get; set; }
    }
}