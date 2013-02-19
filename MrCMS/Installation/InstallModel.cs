using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Installation
{
    public class InstallModel
    {
        [AllowHtml]
        public string AdminEmail { get; set; }

        [AllowHtml]
        [DataType(DataType.Password)]
        public string AdminPassword { get; set; }

        [AllowHtml]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [AllowHtml]
        public string DatabaseConnectionString { get; set; }

        public DatabaseType DatabaseType { get; set; }
        //SQL Server properties
        public string SqlConnectionInfo { get; set; }

        [AllowHtml]
        public string SqlServerName { get; set; }

        [AllowHtml]
        public string SqlDatabaseName { get; set; }

        [AllowHtml]
        public string SqlServerUsername { get; set; }

        [AllowHtml]
        public string SqlServerPassword { get; set; }

        public string SqlAuthenticationType { get; set; }
        public bool SqlServerCreateDatabase { get; set; }

        public string SiteName { get; set; }
        public string SiteUrl { get; set; }

        public Webpage HomePage { get; set; }
        public Webpage Page2 { get; set; }
        public Webpage Page3 { get; set; }
        public Webpage Error404 { get; set; }
        public Webpage Error500 { get; set; }

        public Layout BaseLayout { get; set; }

    }
}