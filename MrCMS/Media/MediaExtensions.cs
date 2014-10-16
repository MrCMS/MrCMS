namespace MrCMS.Media
{
    public static class MediaExtensions
    {
        public static string GetIconClass(this string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                return "";
            switch (extension.ToLower())
            {
                case ".doc":
                case ".docx":
                    return "fa fa-file-word-o";
                case  "xls":
                case ".xlsx":
                case ".csv":
                    return "fa fa-file-excel-o";
                case ".pdf":
                    return "fa fa-file-pdf-o";
                case ".txt":
                    return "fa fa-file-text";
                case ".zip":
                case ".rar":
                case ".7z":
                    return "fa fa-file-archive-o";
                case ".ppt":
                case ".pptx":
                    return "fa fa-file-powerpoint-o";
                case ".mov":
                case ".flv":
                case ".mp4":
                case ".wmv":
                case ".avi":
                case ".mpeg":
                case ".webm":
                case ".ogv":
                    return "fa fa-file-video-o";
                case ".mp3":
                    return "fa fa-music";
                case ".htm":
                case ".html":
                    return "fa fa-html5";
            }

            return "glyphicon glyphicon-file";
        }
    }
}