namespace MrCMS.Website.Optimization
{
    public struct ResourceData
    {
        public static ResourceData Get(bool isRemote, string url)
        {
            return new ResourceData
                {
                    IsRemote = isRemote,
                    Url = url
                };
        }

        public bool IsRemote { get; set; }
        public string Url { get; set; }
    }
}