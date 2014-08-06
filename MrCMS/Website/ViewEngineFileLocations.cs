namespace MrCMS.Website
{
    public static class ViewEngineFileLocations
    {
        static ViewEngineFileLocations()
        {
            //Apps
            AppViewLocationFormats = new[]
            {
                "~/Apps/{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Apps/{3}/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Apps/{3}/Views/{1}/{0}.cshtml",
                "~/Apps/{3}/Views/Shared/{0}.cshtml",
                "~/Apps/{3}/Views/Pages/{0}.cshtml"
            };
            AppMasterLocationFormats = new[]
            {
                "~/Apps/{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Apps/{3}/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Apps/{3}/Views/{1}/{0}.cshtml",
                "~/Apps/{3}/Views/Shared/{0}.cshtml"
            };
            AppPartialViewLocationFormats = new[]
            {
                "~/Apps/{3}/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Apps/{3}/Areas/{2}/Views/Shared/{0}.cshtml",
                "~/Apps/{3}/Views/{1}/{0}.cshtml",
                "~/Apps/{3}/Views/Shared/{0}.cshtml",
                "~/Apps/{3}/Views/Widgets/{0}.cshtml"
            };

            //MVC Default
            AreaViewLocationFormats = new[]
            {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml"
            };
            AreaMasterLocationFormats = new[]
            {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml"
            };
            AreaPartialViewLocationFormats = new[]
            {
                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/Shared/{0}.cshtml"
            };

            ViewLocationFormats = new[]
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Pages/{0}.cshtml"
            };
            MasterLocationFormats = new[]
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };
            PartialViewLocationFormats = new[]
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Widgets/{0}.cshtml"
            };

            FileExtensions = new[]
            {
                "cshtml"
            };
        }

        public static string[] AppMasterLocationFormats { get; private set; }
        public static string[] AppPartialViewLocationFormats { get; private set; }
        public static string[] AppViewLocationFormats { get; private set; }
        public static string[] AreaMasterLocationFormats { get; private set; }
        public static string[] AreaPartialViewLocationFormats { get; private set; }
        public static string[] AreaViewLocationFormats { get; private set; }
        public static string[] FileExtensions { get; private set; }
        public static string[] MasterLocationFormats { get; private set; }
        public static string[] PartialViewLocationFormats { get; private set; }
        public static string[] ViewLocationFormats { get; private set; }
    }
}