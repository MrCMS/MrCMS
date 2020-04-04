using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Installation.Models
{
    public class InstallModel
    {
        public InstallModel()
        {
           
        }

        [Required]
        [EmailAddress]
        [DisplayName("Admin Email")]
        public string AdminEmail { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [DisplayName("Admin Password")]
        [Compare("ConfirmPassword", ErrorMessage = "Passwords must match")]
        public string AdminPassword { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

        //[DisplayName("Database Connection String")]
        //public string DatabaseConnectionString { get; set; }

        //[DisplayName("Database Provider")] public string DatabaseProvider { get; set; }

        ////SQL Server properties
        //[DisplayName("SQL Connection Info")] public SqlConnectionInfo SqlConnectionInfo { get; set; }

        //[DisplayName("SQL Server Name/IP")] public string SqlServerName { get; set; }

        //[DisplayName("SQL Database Name")] public string SqlDatabaseName { get; set; }

        //[DisplayName("SQL Server Username")] public string SqlServerUsername { get; set; }

        //[DisplayName("SQL Server Password")] public string SqlServerPassword { get; set; }

        //[DisplayName("SQL Authentication Type")]
        //public SqlAuthenticationType SqlAuthenticationType { get; set; }

        //[DisplayName("SQL Server Create Database")]
        //public bool SqlServerCreateDatabase { get; set; }

        [Required] [DisplayName("Site Name")] public string SiteName { get; set; }

        [Required]
        [DisplayName("Site URL (www.yourdomain.com)")]
        public string SiteUrl { get; set; }

        [DisplayName("UI Culture")] [Required] public string UiCulture { get; set; }
        [DisplayName("Culture Options")] public IEnumerable<SelectListItem> CultureOptions { get; set; }
    }
}