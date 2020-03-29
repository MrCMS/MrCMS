using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class FormPropertyAdminService : IFormPropertyAdminService
    {
        private readonly IRepository<FormProperty> _repository;
        private readonly IMapper _mapper;

        public FormPropertyAdminService(IRepository<FormProperty> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task Add(AddFormPropertyModel model)
        {
            var type = TypeHelper.GetTypeByName(model.PropertyType);
            if (!(Activator.CreateInstance(type) is FormProperty property))
            {
                return;
            }

            _mapper.Map(model, property);


            property.DisplayOrder = await _repository.Readonly().AnyAsync(x => x.FormId == model.FormId)
                ? await _repository.Readonly().Where(x => x.FormId == model.FormId).MaxAsync(x => x.DisplayOrder) + 1
                : 0;

            await _repository.Add(property);
        }

        public async Task Update(UpdateFormPropertyModel model)
        {
            var property = await GetFormProperty(model.Id);
            _mapper.Map(model, property);
            await _repository.Update(property);
        }

        public async Task Delete(int id)
        {
            var property = await GetFormProperty(id);
            property.Form.FormProperties.Remove(property);
            await _repository.Delete(property);
        }

        public List<SelectListItem> GetPropertyTypeOptions()
        {
            // this is intentionally a static list so that they can be ordered correctly,
            var orderedTypes = new List<Type>
            {
                typeof(TextBox),
                typeof(Email),
                typeof(DropDownList),
                typeof(TextArea),
                typeof(CheckboxList),
                typeof(RadioButtonList),
                typeof(FileUpload)
            };

            // but we still want to load all in case any custom types have been added
            return TypeHelper.GetAllConcreteTypesAssignableFrom<FormProperty>()
                .OrderBy(type => orderedTypes.Contains(type))
                .ThenBy(type => orderedTypes.IndexOf(type))
                .ThenBy(type => type.Name)
                .BuildSelectItemList(type => type.Name.BreakUpString(),
                    type => type.FullName,
                    emptyItemText: null);
        }

        public async Task<UpdateFormPropertyModel> GetUpdateModel(int id)
        {
            var formProperty = await GetFormProperty(id);
            return _mapper.Map<UpdateFormPropertyModel>(formProperty);
        }

        private Task<FormProperty> GetFormProperty(int id)
        {
            return _repository.Load(id);
        }
    }
}