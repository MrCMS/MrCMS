namespace MrCMS.Entities.People
{
    public interface IBelongToUser
    {
        User User { get; set; }
        int UserId { get; set; }
        
    }
}