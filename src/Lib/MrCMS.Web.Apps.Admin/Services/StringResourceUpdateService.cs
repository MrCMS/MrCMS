using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Resources;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class StringResourceUpdateService : IStringResourceUpdateService
    {
        public const string Quote = "\"";
        private readonly IStringResourceProvider _provider;
        private readonly IGlobalRepository<StringResource> _repository;

        public StringResourceUpdateService(IStringResourceProvider provider, IGlobalRepository<StringResource> repository)
        {
            _provider = provider;
            _repository = repository;
        }

        public FileResult Export(StringResourceSearchQuery searchQuery)
        {
            var resources = _provider.AllResources.GetResourcesByKeyAndValue(searchQuery);
            var data = new List<List<string>>
                           {
                               new List<string>
                                   {
                                       "Key",
                                       "Value",
                                       "Culture",
                                       "SiteId"
                                   }
                           };
            foreach (var source in resources.OrderBy(resource => resource.Key))
            {
                var row = new List<string>();
                AddToRow(row, source.Key);
                AddToRow(row, source.Value);
                AddToRow(row, source.UICulture);
                AddToRow(row, source.Site == null ? string.Empty : source.Site.Id.ToString());
                data.Add(row);
            }
            string result = string.Join(Environment.NewLine, data.Select(list => string.Join(",", list)));

            return new FileContentResult(Encoding.UTF8.GetBytes(result), "text/csv")
            {
                FileDownloadName =
                    "MrCMS-ResourceData-" + DateTime.UtcNow +
                    ".csv"
            };
        }

        public ResourceImportSummary Import(IFormFile file)
        {
            Stream inputStream = file.OpenReadStream();
            inputStream.Position = 0;
            var reader = new StreamReader(inputStream, Encoding.UTF8);
            string data = reader.ReadToEnd();
            string[] rows = data.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var resourceData = new List<StringResourceData>();
            foreach (string row in rows.Skip(1)) //skip the header row
            {
                string[] columns = row.Split(new[] { "," }, StringSplitOptions.None);
                if (columns.Count() != 4)
                    continue;
                try
                {
                    string resourceKey = ReadData(columns[0]);

                    int id;
                    resourceData.Add(new StringResourceData
                    {
                        Key = resourceKey,
                        Value = ReadData(columns[1]),
                        UICulture = ReadData(columns[2]),
                        SiteId = int.TryParse(ReadData(columns[3]), out id) ? id : (int?)null
                    });
                }
                catch
                {

                }
            }
            int added = 0, updated = 0;
            foreach (StringResourceData stringResourceData in resourceData)
            {
                string uiCulture = string.IsNullOrWhiteSpace(stringResourceData.UICulture)
                                       ? null
                                       : stringResourceData.UICulture;
                StringResource stringResource =
                    _provider.AllResources.FirstOrDefault(
                        resource =>
                        resource.Key == stringResourceData.Key &&
                        (stringResourceData.SiteId.HasValue
                        ? resource.Site != null && resource.Site.Id == stringResourceData.SiteId
                        : resource.Site == null) &&
                        resource.UICulture == uiCulture);
                if (stringResource != null)
                {
                    // we need to load the one out of the current session or else we'll have issues when saving
                    stringResource = _repository.LoadSync(stringResource.Id);
                    stringResource.Value = stringResourceData.Value;
                    _provider.Update(stringResource);
                    updated++;
                }
                else
                {
                    _provider.Insert(new StringResource
                    {
                        Key = stringResourceData.Key,
                        Value = stringResourceData.Value,
                        UICulture = uiCulture,
                        SiteId = stringResourceData.SiteId
                    });
                    added++;
                }
            }
            return new ResourceImportSummary
            {
                Added = added,
                Updated = updated
            };
        }

        private void AddToRow(List<string> row, string data)
        {
            row.Add(string.Format("{1}{0}{1}", (data ?? string.Empty).Replace(Quote, Quote + Quote), Quote));
        }

        private string ReadData(string value)
        {
            value ??= string.Empty;

            value = value.Trim('"');
            value = value.Replace("\"\"", "\"");
            return value;
        }

        public class StringResourceData
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public string UICulture { get; set; }
            public int? SiteId { get; set; }
        }
    }
}