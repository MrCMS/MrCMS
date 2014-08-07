using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.Installation
{
    public class InstallModel
    {
        [AllowHtml]
        [Required]
        [EmailAddress]
        [DisplayName("Admin Email")]
        public string AdminEmail { get; set; }

        [AllowHtml]
        [DataType(DataType.Password)]
        [Required]
        [DisplayName("Admin Password")]
        [System.ComponentModel.DataAnnotations.Compare("ConfirmPassword", ErrorMessage = "Passwords must match")]
        public string AdminPassword { get; set; }

        [AllowHtml]
        [DataType(DataType.Password)]
        [Required]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

        [AllowHtml]
        [DisplayName("Database Connection String")]
        public string DatabaseConnectionString { get; set; }

        public DatabaseType DatabaseType { get; set; }
        //SQL Server properties
        [DisplayName("SQL Connection Info")]
        public string SqlConnectionInfo { get; set; }

        [AllowHtml]
        [DisplayName("SQL Server Name/IP")]
        public string SqlServerName { get; set; }

        [AllowHtml]
        [DisplayName("SQL Database Name")]
        public string SqlDatabaseName { get; set; }

        [AllowHtml]
        [DisplayName("SQL Server Username")]
        public string SqlServerUsername { get; set; }

        [AllowHtml]
        [DisplayName("SQL Server Password")]
        public string SqlServerPassword { get; set; }

        [DisplayName("SQL Authentication Type")]
        public string SqlAuthenticationType { get; set; }

        [DisplayName("SQL Server Create Database")]
        public bool SqlServerCreateDatabase { get; set; }

        [Required]
        [DisplayName("Site Name")]
        public string SiteName { get; set; }

        [Required]
        [DisplayName("Site URL (www.yourdomain.com)")]
        public string SiteUrl { get; set; }

        [DisplayName("UI Culture")]
        public string UiCulture { get; set; }
        [DisplayName("Time Zone")]
        public string TimeZone { get; set; }

        [DisplayName("Culture Options")]
        public IEnumerable<SelectListItem> CultureOptions
        {
            get
            {
                return CultureInfo.GetCultures(CultureTypes.AllCultures).OrderBy(info => info.DisplayName)
                           .BuildSelectItemList(info => info.DisplayName, info => info.Name,
                                                info =>
                                                string.IsNullOrWhiteSpace(UiCulture)
                                                    ? Equals(info, CultureInfo.CurrentCulture)
                                                    : info.Name == UiCulture,
                                                emptyItem: null);
            }
        }

        public IEnumerable<SelectListItem> TimeZoneOptions
        {
            get
            {
                return TimeZoneInfo.GetSystemTimeZones()
                                   .BuildSelectItemList(info => info.DisplayName, info => info.Id,
                                                        info =>
                                                        string.IsNullOrWhiteSpace(TimeZone)
                                                            ? Equals(info, TimeZoneInfo.Local)
                                                            : info.Id == TimeZone, emptyItem: null);
            }
        }

        public Webpage HomePage { get; set; }
        public Webpage Page2 { get; set; }
        public Webpage Page3 { get; set; }
        public Webpage Error403 { get; set; }
        public Webpage Error404 { get; set; }
        public Webpage Error500 { get; set; }
        public Layout BaseLayout { get; set; }
    }
}