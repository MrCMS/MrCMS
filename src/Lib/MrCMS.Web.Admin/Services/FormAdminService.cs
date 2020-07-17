using AutoMapper;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services.Resources;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class FormAdminService : IFormAdminService
    {
        private readonly ISession _session;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly ILogger<FormAdminService> _logger;
        private readonly IMapper _mapper;
        private readonly IGetCurrentUserCultureInfo _getCurrentUserCultureInfo;

        public FormAdminService(ISession session, IStringResourceProvider stringResourceProvider, ILogger<FormAdminService> logger,
            IMapper mapper, IGetCurrentUserCultureInfo getCurrentUserCultureInfo)
        {
            _session = session;
            _stringResourceProvider = stringResourceProvider;
            _logger = logger;
            _mapper = mapper;
            _getCurrentUserCultureInfo = getCurrentUserCultureInfo;
        }

        public void ClearFormData(Form form)
        {
            _session.Transact(session =>
            {
                form.FormPostings.ForEach(session.Delete);
                form.FormPostings.Clear();
            });
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

        public FormPosting DeletePosting(int id)
        {
            var posting = _session.Get<FormPosting>(id);
            posting.Form.FormPostings.Remove(posting);
            _session.Transact(session => session.Delete(posting));
            return posting;
        }

        public void SetOrders(List<SortItem> items)
        {
            _session.Transact(session => items.ForEach(item =>
            {
                var formItem = session.Get<FormProperty>(item.Id);
                formItem.DisplayOrder = item.Order;
                session.Update(formItem);
            }));
        }

        public IPagedList<Form> Search(FormSearchModel model)
        {
            var query = _session.Query<Form>();

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                query = query.Where(x => x.Name.Contains(model.Name));
            }

            return query.ToPagedList(model.Page);
        }

        public Form AddForm(AddFormModel model)
        {
            var form = _mapper.Map<Form>(model);

            _session.Transact(session => session.Save(form));

            return form;
        }

        public Form GetForm(int id)
        {
            return _session.Get<Form>(id);
        }

        public UpdateFormModel GetUpdateModel(int id)
        {
            var form = GetForm(id);

            return _mapper.Map<UpdateFormModel>(form);
        }

        public void Update(UpdateFormModel model)
        {
            var form = GetForm(model.Id);
            _mapper.Map(model, form);

            foreach (var o in model.Models)
            {
                _mapper.Map(o, form);
            }

            _session.Transact(session => session.Update(form));
        }

        public void Delete(int id)
        {
            var form = GetForm(id);

            _session.Transact(session => session.Delete(form));
        }

        public PostingsModel GetFormPostings(Form form, int page, string search)
        {
            FormPosting posting = null;
            var query =
                _session.QueryOver(() => posting).Where(() => posting.Form.Id == form.Id);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query =
                    query.WithSubquery.WhereExists(
                        QueryOver.Of<FormValue>()
                            .Where(
                                value =>
                                    value.FormPosting.Id == posting.Id &&
                                    value.Value.IsInsensitiveLike(search, MatchMode.Anywhere)).Select(value => value.Id));
            }

            IPagedList<FormPosting> formPostings = query.OrderBy(() => posting.CreatedOn).Desc.Paged(page);

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