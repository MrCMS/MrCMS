namespace MrCMS.Entities.People
{
    public class UserLogin : SystemEntity
    {
        public virtual string LoginProvider { get; set; }

        public virtual string ProviderKey { get; set; }

        public virtual User User { get; set; }
    }
}