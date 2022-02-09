namespace MrCMS.Entities.People
{
    public class UserToken : SystemEntity
    {
        public virtual string LoginProvider { get; set; }
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }

        public virtual User User { get; set; }
    }
}