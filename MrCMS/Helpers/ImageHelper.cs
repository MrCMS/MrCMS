using System.Drawing;

namespace MrCMS.Helpers
{
    public static class ImageHelper
    {
        public static bool IsSmallerThan(this Size size1, Size size2)
        {
            return size2.Width > size1.Width && size2.Height > size1.Height;
        }
    }
}