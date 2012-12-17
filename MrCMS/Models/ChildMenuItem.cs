namespace MrCMS.Models
{
    public class ChildMenuItem : IMenuItem
    {
        public ChildMenuItem(string text, string url)
        {
            Url = url;
            Text = text;
        }

        public string Text { get; private set; }
        public string Url { get; private set; }
    }
}