using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class FormPropertyAdminService : IFormPropertyAdminService
    {
        private readonly ISession _session;
        private readonly IMapper _mapper;

        public FormPropertyAdminService(ISession session, IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }

        public void Add(AddFormPropertyModel model)
        {
            var type = TypeHelper.GetTypeByName(model.PropertyType);
            var property = Activator.CreateInstance(type) as FormProperty;
            if (property == null)
            {
                return;
            }

            _mapper.Map(model, property);

            property.Form?.FormProperties.Add(property);

            _session.Transact(session =>
            {
                if (property.Form.FormProperties != null)
                {
                    property.DisplayOrder = property.Form.FormProperties.Count;
                }

                session.Save(property);
            });
        }

        public void Update(UpdateFormPropertyModel model)
        {
            var property = GetFormProperty(model.Id);
            _mapper.Map(model, property);
            _session.Transact(session => session.Update(property));
        }

        public void Delete(int id)
        {
            var property = GetFormProperty(id);
            property.Form.FormProperties.Remove(property);
            _session.Transact(session => session.Delete(property));
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

        public UpdateFormPropertyModel GetUpdateModel(int id)
        {
            var formProperty = GetFormProperty(id);
            return _mapper.Map<UpdateFormPropertyModel>(formProperty);
        }

        private FormProperty GetFormProperty(int id)
        {
            return _session.Get<FormProperty>(id);
        }
    }
}