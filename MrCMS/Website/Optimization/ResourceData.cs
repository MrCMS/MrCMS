namespace MrCMS.Website.Optimization
{
    public class ResourceData
    {
        public ResourceData(bool isRemote, string url)
        {
            IsRemote = isRemote;
            Url = url;
        }

        public bool IsRemote { get; set; }
        public string Url { get; set; }
    }
}