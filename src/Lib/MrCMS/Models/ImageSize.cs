using System.Drawing;

namespace MrCMS.Models
{
    public class ImageSize
    {
        public ImageSize() { }

        public ImageSize(string name, Size size)
        {
            Name = name;
            Size = size;
            ActualSize = size;
        }

        public string Name { get; set; }

        public Size Size { get; set; }
        public int Height => Size.Height;
        public int Width => Size.Width;

        public Size ActualSize { get; set; }
        public int ActualHeight => ActualSize.Height;
        public int ActualWidth => ActualSize.Width;
    }
}