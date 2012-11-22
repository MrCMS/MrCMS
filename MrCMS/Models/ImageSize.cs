using System.Drawing;

namespace MrCMS.Models
{
    public class ImageSize
    {
        public string Name { get; set; }

        public Size Size { get; set; }
        public int Height { get { return Size.Height; } }
        public int Width { get { return Size.Width; } }

        public Size ActualSize { get; set; }
        public int ActualHeight { get { return ActualSize.Height; } }
        public int ActualWidth { get { return ActualSize.Width; } }
    }
}