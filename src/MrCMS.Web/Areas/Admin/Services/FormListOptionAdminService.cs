using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Web.Areas.Admin.Models;
using IMapper = AutoMapper.IMapper;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class FormListOptionAdminService : IFormListOptionAdminService
    {
        private readonly IRepository<FormListOption> _repository;
        private readonly IMapper _mapper;

        public FormListOptionAdminService(IRepository<FormListOption> repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task Add(AddFormListOptionModel model)
        {
            var formListOption = _mapper.Map<FormListOption>(model);

            FormPropertyWithOptions formProperty = formListOption.FormProperty;
            formProperty?.Options.Add(formListOption);

            await formListOption.OnSaving(_repository);
            await _repository.Add(formListOption);
        }

        private Task<FormListOption> GetFormListOption(int id)
        {
            return _repository.Load(id);
        }

        public async Task Update(UpdateFormListOptionModel model)
        {
            var formListOption = await GetFormListOption(model.Id);
            _mapper.Map(model, formListOption);
            await formListOption.OnSaving(_repository);
            await _repository.Update(formListOption);
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
            await _repository.Delete(formListOption);
        }

        public AddFormListOptionModel GetAddModel(int formPropertyId)
        {
            return new AddFormListOptionModel { FormPropertyId = formPropertyId };
        }

        public async Task<UpdateFormListOptionModel> GetUpdateModel(int id)
        {
            return _mapper.Map<UpdateFormListOptionModel>(await GetFormListOption(id));
        }
    }
}