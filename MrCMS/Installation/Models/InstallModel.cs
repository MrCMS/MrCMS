using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Helpers;
using MrCMS.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace MrCMS.Installation.Models
{
    public class InstallModel
    {
        public InstallModel()
        {
            TimeZone = (TimeZones.Zones.FirstOrDefault(x => Equals(x, TimeZoneInfo.Local))
                        ?? TimeZones.Zones.FirstOrDefault(x => x.BaseUtcOffset == TimeZoneInfo.Local.BaseUtcOffset))
                ?.ToSerializedString();
        }

        [Required]
        [EmailAddress]
        [DisplayName("Admin Email")]
        public string AdminEmail { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [DisplayName("Admin Password")]
        [System.ComponentModel.DataAnnotations.Compare("ConfirmPassword", ErrorMessage = "Passwords must match")]
        public string AdminPassword { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Database Connection String")]
        public string DatabaseConnectionString { get; set; }

        [DisplayName("Database Provider")]
        public string DatabaseProvider { get; set; }

        //SQL Server properties
        [DisplayName("SQL Connection Info")]
        public SqlConnectionInfo SqlConnectionInfo { get; set; }

        [DisplayName("SQL Server Name/IP")]
        public string SqlServerName { get; set; }

        [DisplayName("SQL Database Name")]
        public string SqlDatabaseName { get; set; }

        [DisplayName("SQL Server Username")]
        public string SqlServerUsername { get; set; }

        [DisplayName("SQL Server Password")]
        public string SqlServerPassword { get; set; }

        [DisplayName("SQL Authentication Type")]
        public SqlAuthenticationType SqlAuthenticationType { get; set; }

        [DisplayName("SQL Server Create Database")]
        public bool SqlServerCreateDatabase { get; set; }

        [Required]
        [DisplayName("Site Name")]
        public string SiteName { get; set; }

        [Required]
        [DisplayName("Site URL (www.yourdomain.com)")]
        public string SiteUrl { get; set; }

        [DisplayName("UI Culture"), Required]
        public string UiCulture { get; set; }
        [DisplayName("Time Zone"), Required]
        public string TimeZone { get; set; }

        [DisplayName("Culture Options")]
        public IEnumerable<SelectListItem> CultureOptions => CultureInfo.GetCultures(CultureTypes.AllCultures).OrderBy(info => info.DisplayName)
                           .BuildSelectItemList(info => info.DisplayName, info => info.Name,
                                                info =>
                                                string.IsNullOrWhiteSpace(UiCulture)
                                                    ? Equals(info, CultureInfo.CurrentCulture)
                                                    : info.Name == UiCulture,
                                                emptyItem: null);

        public IEnumerable<SelectListItem> TimeZoneOptions => TimeZones.Zones
                                   .BuildSelectItemList(info => info.DisplayName, info => info.ToSerializedString(),
                                                        info =>
                                                        string.IsNullOrWhiteSpace(TimeZone)
                                                            ? Equals(info, TimeZoneInfo.Local)
                                                            : info.ToSerializedString() == TimeZone, emptyItem: null);
    }
}