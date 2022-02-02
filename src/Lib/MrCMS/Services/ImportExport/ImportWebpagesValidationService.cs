using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Services.ImportExport.Rules;

namespace MrCMS.Services.ImportExport
{
    public class ImportWebpagesValidationService : IImportWebpagesValidationService
    {
        private readonly IWebpageUrlService _webpageUrlService;
        private readonly IServiceProvider _serviceProvider;

        public ImportWebpagesValidationService(IWebpageUrlService webpageUrlService, IServiceProvider serviceProvider)
        {
            _webpageUrlService = webpageUrlService;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Validate Business Logic
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, List<string>>> ValidateBusinessLogic(IEnumerable<WebpageImportDTO> items)
        {
            var errors = new Dictionary<string, List<string>>();
            var itemRules = _serviceProvider.GetServices<IWebpageImportValidationRule>();

            var documentImportDataTransferObjects = items as IList<WebpageImportDTO> ?? items.ToList();
            foreach (var item in documentImportDataTransferObjects)
            {
                var validationErrors = new List<string>();
                foreach (var rule in itemRules)
                {
                    validationErrors.AddRange(await rule.GetErrors(item, documentImportDataTransferObjects));
                }

                if (validationErrors.Any())
                    errors.Add(item.UrlSegment, validationErrors);
            }

            return errors;
        }

        /// <summary>
        /// Parse and Import to DTOs
        /// </summary>
        /// <param name="spreadsheet"></param>
        /// <returns></returns>
        public async Task<(List<WebpageImportDTO> data, Dictionary<string, List<string>> parseErrors)>
            ValidateAndImportDocuments(XLWorkbook spreadsheet)
        {
            Dictionary<string, List<string>> parseErrors = new Dictionary<string, List<string>>();
            var items = new List<WebpageImportDTO>();

            if (spreadsheet != null)
            {
                var worksheet = spreadsheet.Worksheets.SingleOrDefault(x => x.Name == "Items");
                if (worksheet == null)
                {
                    return (items, parseErrors);
                }

                var totalRows = worksheet.RowCount();
                for (var rowId = 2; rowId <= totalRows; rowId++)
                {
                    //Prepare handle name for storing and grouping errors
                    var urlSegment = worksheet.Cell(rowId, 1).GetValue<string>();
                    var name = worksheet.Cell(rowId, 4).GetValue<string>();
                    var handle = urlSegment.HasValue() ? urlSegment : name;

                    if (string.IsNullOrWhiteSpace(handle) || items.Any(x => x.UrlSegment == urlSegment))
                        continue;

                    List<string> errors = parseErrors.ContainsKey(handle)
                        ? parseErrors[handle]
                        : new List<string>();

                    var (item, itemErrors) = await GetDocumentImportDataTransferObject(worksheet, rowId, name);
                    errors.AddRange(itemErrors);
                    parseErrors[handle] = errors;

                    items.Add(item);
                }

                //Remove duplicate errors
                parseErrors = parseErrors.GroupBy(x => x.Value)
                    .Select(x => x.First())
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            //Remove handles with no errors
            parseErrors = parseErrors.Where(x => x.Value.Any()).ToDictionary(pair => pair.Key, pair => pair.Value);

            return (items, parseErrors);
        }

        private async Task<(WebpageImportDTO item, List<string> errors)> GetDocumentImportDataTransferObject(
            IXLWorksheet worksheet, int rowId,
            string name)
        {
            List<string> parseErrors = new List<string>();
            var item = new WebpageImportDTO();
            item.ParentUrl = worksheet.Cell(rowId, 2).GetValue<string>();
            if (worksheet.Cell(rowId, 3).GetValue<string>().HasValue())
            {
                item.WebpageType = worksheet.Cell(rowId, 3).GetValue<string>();
                item.UrlSegment = worksheet.Cell(rowId, 1).GetValue<string>().HasValue()
                    ? worksheet.Cell(rowId, 1).GetValue<string>()
                    : await _webpageUrlService.Suggest(new SuggestParams
                        {PageName = name, WebpageType = item.WebpageType});
            }
            else
                parseErrors.Add("Document Type is required.");

            if (worksheet.Cell(rowId, 4).GetValue<string>().HasValue())
                item.Name = worksheet.Cell(rowId, 4).GetValue<string>();
            else
                parseErrors.Add("Document Name is required.");
            item.BodyContent = worksheet.Cell(rowId, 5).GetValue<string>();
            item.MetaTitle = worksheet.Cell(rowId, 6).GetValue<string>();
            item.MetaDescription = worksheet.Cell(rowId, 7).GetValue<string>();
            item.MetaKeywords = worksheet.Cell(rowId, 8).GetValue<string>();
            item.Tags = GetTags(worksheet, rowId, parseErrors);
            if (worksheet.Cell(rowId, 10).GetValue<string>().HasValue())
            {
                if (!worksheet.Cell(rowId, 10).GetValue<string>().IsValidInput<bool>())
                    parseErrors.Add("Reveal in Navigation is not a valid boolean value.");
                else
                    item.RevealInNavigation = worksheet.Cell(rowId, 10).GetValue<bool>();
            }
            else
                item.RevealInNavigation = false;

            if (worksheet.Cell(rowId, 11).GetValue<string>().HasValue())
            {
                if (!worksheet.Cell(rowId, 11).GetValue<string>().IsValidInput<int>())
                    parseErrors.Add("Display Order is not a valid number.");
                else
                    item.DisplayOrder = worksheet.Cell(rowId, 11).GetValue<int>();
            }
            else
                item.DisplayOrder = 0;


            if (worksheet.Cell(rowId, 12).GetValue<string>().HasValue())
            {
                if (!worksheet.Cell(rowId, 12).GetValue<string>().IsValidInputDateTime())
                    parseErrors.Add("Publish Date is not a valid date.");
                else
                    item.PublishDate = worksheet.Cell(rowId, 12).GetValue<DateTime>();
            }

            item.UrlHistory = GetUrlHistory(worksheet, rowId, parseErrors);
            return (item, parseErrors);
        }

        private static List<string> GetUrlHistory(IXLWorksheet worksheet, int rowId, List<string> parseErrors)
        {
            var list = new List<String>();
            try
            {
                var value = worksheet.Cell(rowId, 13).GetValue<string>();
                if (!String.IsNullOrWhiteSpace(value))
                {
                    var urls = value.Split(',');
                    foreach (var url in urls.Where(url => !String.IsNullOrWhiteSpace(url)))
                    {
                        list.Add(url);
                    }
                }
            }
            catch (Exception)
            {
                parseErrors.Add("Url History field value contains illegal characters / not in correct format.");
            }

            return list;
        }

        private static List<string> GetTags(IXLWorksheet worksheet, int rowId, List<string> parseErrors)
        {
            List<string> tagList = new List<string>();
            try
            {
                var value = worksheet.Cell(rowId, 9).GetValue<string>();
                if (!String.IsNullOrWhiteSpace(value))
                {
                    var tags = value.Split(',');
                    foreach (var tag in tags.Where(tag => !String.IsNullOrWhiteSpace(tag)))
                    {
                        tagList.Add(tag);
                    }
                }
            }
            catch (Exception)
            {
                parseErrors.Add(
                    "Url History field value contains illegal characters / not in correct format.");
            }

            return tagList;
        }

        /// <summary>
        /// Validate Import File
        /// </summary>
        /// <param name="spreadsheet"></param>
        /// <returns></returns>
        public Dictionary<string, List<string>> ValidateImportFile(XLWorkbook spreadsheet)
        {
            var parseErrors = new Dictionary<string, List<string>> {{"file", new List<string>()}};

            if (spreadsheet == null)
                parseErrors["file"].Add("No import file");
            else
            {
                if (spreadsheet.Worksheets.Count == 0)
                    parseErrors["file"].Add("No worksheets in import file.");
                else
                {
                    if (spreadsheet.Worksheets.Count < 2 ||
                        !spreadsheet.Worksheets.Any(x => x.Name == "Info") ||
                        !spreadsheet.Worksheets.Any(x => x.Name == "Items"))
                        parseErrors["file"].Add(
                            "One or both of the required worksheets (Info and Items) are not present in import file.");
                }
            }

            return parseErrors.Where(x => x.Value.Any()).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}