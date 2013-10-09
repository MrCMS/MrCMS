namespace MrCMS.Models
{
    public class UserSearchQuery
    {
        public UserSearchQuery()
        {
            Page = 1;
        }
        public string Query { get; set; }
        public int Page { get; set; }
    }
}