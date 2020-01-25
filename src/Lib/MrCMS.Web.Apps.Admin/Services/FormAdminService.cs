using AutoMapper;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Admin.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class FormAdminService : IFormAdminService
    {
        private readonly IRepository<Form> _formRepository;
        private readonly IRepository<FormPosting> _formPostingRepository;
        private readonly IRepository<FormProperty> _formPropertyRepository;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly ILogger<FormAdminService> _logger;
        private readonly IMapper _mapper;
        private readonly IGetCurrentUserCultureInfo _getCurrentUserCultureInfo;

        public FormAdminService(IRepository<Form> formRepository,
            IRepository<FormPosting> formPostingRepository,
            IRepository<FormProperty> formPropertyRepository,
             IStringResourceProvider stringResourceProvider, ILogger<FormAdminService> logger,
            IMapper mapper, IGetCurrentUserCultureInfo getCurrentUserCultureInfo)
        {
            _formRepository = formRepository;
            _formPostingRepository = formPostingRepository;
            _formPropertyRepository = formPropertyRepository;
            _stringResourceProvider = stringResourceProvider;
            _logger = logger;
            _mapper = mapper;
            _getCurrentUserCultureInfo = getCurrentUserCultureInfo;
        }

        public async Task ClearFormData(Form form)
        {
            await _formPostingRepository.DeleteRange(form.FormPostings);
            form.FormPostings.Clear();
            //_session.Transact(session =>
            //{
            //    form.FormPostings.ForEach(session.Delete);
            //    form.FormPostings.Clear();
            //});
        }

        public byte[] ExportFormData(Form form)
        {
            try
            {
                var stringBuilder = new StringBuilder();

                var headers = GetHeadersForExport(form).ToList();
                stringBuilder.AppendLine(string.Join(",", headers.Select(FormatField)));

                var formDataForExport = GetFormDataForExport(form);
                foreach (var data in formDataForExport)
                {
                    stringBuilder.AppendLine(string.Join(",", data.Value.Select(FormatField)));
                }

                return Encoding.Default.GetBytes(stringBuilder.ToString());
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                throw;
            }
        }

        public async Task<FormPosting> DeletePosting(int id)
        {
            var posting = await _formPostingRepository.Load(id);
            posting.Form.FormPostings.Remove(posting);
            await _formPostingRepository.Delete(posting);
            return posting;
        }

        public async Task SetOrders(List<SortItem> items)
        {
            var properties = items.Select(item =>
            {
                var formItem = _formPropertyRepository.LoadSync(item.Id);
                formItem.DisplayOrder = item.Order;
                return formItem;
            }).ToList();
            await _formPropertyRepository.UpdateRange(properties);
        }

        public IPagedList<Form> Search(FormSearchModel model)
        {
            var query = _formRepository.Query<Form>();

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                query = query.Where(x => x.Name.Contains(model.Name));
            }

            return query.ToPagedList(model.Page);
        }

        public async Task<Form> AddForm(AddFormModel model)
        {
            var form = _mapper.Map<Form>(model);

            await _formRepository.Add(form);

            return form;
        }

        public Form GetForm(int id)
        {
            return _formRepository.LoadSync(id);
        }

        public UpdateFormModel GetUpdateModel(int id)
        {
            var form = GetForm(id);

            return _mapper.Map<UpdateFormModel>(form);
        }

        public Task<List<FormProperty>> GetProperties(int formId)
        {
            return _formPropertyRepository.Query().Where(x => x.FormId == formId).OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<SortItem>> GetSortItems(int formId)
        {
            return (await GetProperties(formId)).Select(x => new SortItem
            {
                Name = x.Name, Id = x.Id, Order = x.DisplayOrder
            }).ToList();
        }

        public async Task Update(UpdateFormModel model)
        {
            var form = GetForm(model.Id);
            _mapper.Map(model, form);

            foreach (var o in model.Models)
            {
                _mapper.Map(o, form);
            }

            await _formRepository.Update(form);
        }

        public async Task Delete(int id)
        {
            var form = GetForm(id);

            await _formRepository.Delete(form);
        }

        public PostingsModel GetFormPostings(Form form, int page, string search)
        {
            var query = _formPostingRepository.Query().Where(x => x.FormId == form.Id);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query =
                    query.Where(posting =>
                        posting.FormValues.Any(value => EF.Functions.Like(value.Value, $"%{search}%")));
                //query.WithSubquery.WhereExists(
                //    QueryOver.Of<FormValue>()
                //        .Where(
                //            value =>
                //                value.FormPosting.Id == posting.Id &&
                //                value.Value.IsInsensitiveLike(search, MatchMode.Anywhere)).Select(value => value.Id));
            }

            IPagedList<FormPosting> formPostings = query.OrderByDescending(x => x.CreatedOn).ToPagedList(page);

            return new PostingsModel(formPostings, form.Id);
        }

        private IEnumerable<string> GetHeadersForExport(Form form)
        {
            var headers = new List<string>();
            foreach (FormPosting posting in form.FormPostings)
            {
                headers.AddRange(posting.FormValues.Select(x => x.Key).Distinct());
            }
            headers.Add(_stringResourceProvider.GetValue("Admin Form Postings Posted On", "Posted On"));
            return headers.Distinct().ToList();
        }

        private Dictionary<int, List<string>> GetFormDataForExport(Form form)
        {
            var items = new Dictionary<int, List<string>>();
            var formatProvider = _getCurrentUserCultureInfo.Get();
            for (int i = 0; i < form.FormPostings.Count; i++)
            {
                FormPosting posting = form.FormPostings[i];
                items.Add(i, new List<string>());
                foreach (
                    FormValue value in
                        GetHeadersForExport(form)
                            .SelectMany(header => posting.FormValues.Where(x => x.Key == header)))
                {
                    if (!value.IsFile)
                    {
                        items[i].Add(value.Value);
                    }
                    else
                    {
                        items[i].Add("http://" + form.Site.BaseUrl + value.Value);
                    }
                }

                items[i].Add(posting.CreatedOn.ToString(formatProvider));
            }
            return items.OrderByDescending(x => x.Value.Count).ToDictionary(pair => pair.Key, pair => pair.Value);
        }


        private string FormatField(string data)
        {
            return string.Format("{1}{0}{1}", (data ?? string.Empty).Replace(Quote, Quote + Quote), Quote);
        }
        public const string Quote = "\"";
    }
}