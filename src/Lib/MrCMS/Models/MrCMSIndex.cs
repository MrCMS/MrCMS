namespace MrCMS.Models
{
    public class MrCMSIndex
    {
        public string Name { get; set; }
        public bool DoesIndexExist { get; set; }
        public int? NumberOfDocs { get; set; }
        public string TypeName { get; set; }
    }
}