using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Mapping;
using MrCMS.Web.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Admin.Services
{
    public class FormListOptionAdminService : IFormListOptionAdminService
    {
        private readonly ISession _session;
        private readonly ISessionAwareMapper _mapper;

        public FormListOptionAdminService(ISession session, ISessionAwareMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }

        public async Task Add(AddFormListOptionModel model)
        {
            var formListOption = _mapper.Map<FormListOption>(model);

            FormPropertyWithOptions formProperty = formListOption.FormProperty;
            formProperty?.Options.Add(formListOption);

            await _session.TransactAsync(async session =>
            {
                await formListOption.OnSaving(session);
                await session.SaveAsync(formListOption);
            });
        }

        private async Task<FormListOption> GetFormListOption(int id)
        {
            return await _session.GetAsync<FormListOption>(id);
        }

        public async Task Update(UpdateFormListOptionModel model)
        {
            var formListOption = await GetFormListOption(model.Id);
            _mapper.Map(model, formListOption);
            await _session.TransactAsync(async session =>
            {
                await formListOption.OnSaving(session);
                await session.UpdateAsync(formListOption);
            });
        }

        public async Task Delete(int id)
        {
            var formListOption = await GetFormListOption(id);
            if (formListOption == null)
            {
                return;
            }

            FormPropertyWithOptions formProperty = formListOption.FormProperty;
            formProperty?.Options.Remove(formListOption);
            await _session.TransactAsync(session => session.DeleteAsync(formListOption));
        }

        public AddFormListOptionModel GetAddModel(int formPropertyId)
        {
            return new AddFormListOptionModel { FormPropertyId = formPropertyId };
        }

        public async Task<UpdateFormListOptionModel> GetUpdateModel(int id)
        {
            return _mapper.Map<UpdateFormListOptionModel>(await GetFormListOption(id));
        }

        public async Task<bool> CheckValueIsNotEnteredAdd(string value, int formPropertyId)
        {
            return (await _session.QueryOver<FormListOption>().Where(f => f.Value == value && f.FormProperty.Id == formPropertyId).RowCountAsync()) == 0;
        }

        public async Task<bool> CheckValueIsNotEnteredEdit(string value, int id)
        {
            var formListOption = await GetFormListOption(id);
            return (await _session.QueryOver<FormListOption>().Where(f => f.Value == value && f.Id != id && f.FormProperty.Id == formListOption.FormProperty.Id).RowCountAsync()) == 0;
        }
    }
}