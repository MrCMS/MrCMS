using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;
using NHibernate;
using IMapper = AutoMapper.IMapper;

namespace MrCMS.Web.Admin.Services
{
    public class FormListOptionAdminService : IFormListOptionAdminService
    {
        private readonly ISession _session;
        private readonly IMapper _mapper;

        public FormListOptionAdminService(ISession session, IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }
        public void Add(AddFormListOptionModel model)
        {
            var formListOption = _mapper.Map<FormListOption>(model);

            FormPropertyWithOptions formProperty = formListOption.FormProperty;
            formProperty?.Options.Add(formListOption);

            _session.Transact(session =>
            {
                formListOption.OnSaving(session);
                session.Save(formListOption);
            });
        }

        private FormListOption GetFormListOption(int id)
        {
            return _session.Get<FormListOption>(id);
        }

        public void Update(UpdateFormListOptionModel model)
        {
            var formListOption = GetFormListOption(model.Id);
            _mapper.Map(model, formListOption);
            _session.Transact(session =>
            {
                formListOption.OnSaving(session);
                session.Update(formListOption);
            });
        }

        public void Delete(int id)
        {
            var formListOption = GetFormListOption(id);
            if (formListOption == null)
            {
                return;
            }

            FormPropertyWithOptions formProperty = formListOption.FormProperty;
            formProperty?.Options.Remove(formListOption);
            _session.Transact(session => session.Delete(formListOption));
        }

        public AddFormListOptionModel GetAddModel(int formPropertyId)
        {
            return new AddFormListOptionModel { FormPropertyId = formPropertyId };
        }

        public UpdateFormListOptionModel GetUpdateModel(int id)
        {
            return _mapper.Map<UpdateFormListOptionModel>(GetFormListOption(id));
        }
    }
}