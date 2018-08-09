namespace MrCMS.Models
{
    public class ChildMenuItem
    {
        public ChildMenuItem(string text, string url, bool canShow = false, SubMenu subMenu = null)
        {
            Children = subMenu;
            Url = url;
            CanShow = canShow;
            Text = text;
        }

        public string Text { get; }
        public string Url { get; }

        public bool CanShow { get; set; }

        public SubMenu Children { get; }
    }
}