namespace MrCMS.Entities.People
{
    public class UserClaim : SystemEntity
    {
        public virtual string Issuer { get; set; }
        public virtual string Claim { get; set; }
        public virtual string Value { get; set; }

        public virtual User User { get; set; }
    }
}