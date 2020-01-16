using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Admin.Models;


namespace MrCMS.Web.Apps.Admin.Services
{
    public class WidgetAdminService : IWidgetAdminService
    {
        private readonly IRepository<Widget> _repository;
        private readonly IMapper _mapper;

        public WidgetAdminService(IRepository<Widget> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public object GetAdditionalPropertyModel(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return null;

            var documentType = TypeHelper.GetTypeByName(type);
            if (documentType == null)
                return null;
            var additionalPropertyType =
                TypeHelper.GetAllConcreteTypesAssignableFrom(
                    typeof(IAddPropertiesViewModel<>).MakeGenericType(documentType)).FirstOrDefault();
            if (additionalPropertyType == null)
                return null;

            return Activator.CreateInstance(additionalPropertyType);
        }

        public async Task<Widget> AddWidget(AddWidgetModel model, object additionalPropertyModel)
        {
            var type = TypeHelper.GetTypeByName(model.WidgetType);
            var instance = Activator.CreateInstance(type) as Widget;
            _mapper.Map(model, instance);
            if (additionalPropertyModel != null)
                _mapper.Map(additionalPropertyModel, instance);

            instance.LayoutArea.AddWidget(instance);
            await _repository.Add(instance);

            return instance;
        }

        public UpdateWidgetModel GetEditModel(int id)
        {
            return _mapper.Map<UpdateWidgetModel>(GetWidget(id));
        }

        public Widget GetWidget(int id)
        {
            return _repository.LoadSync(id);
        }

        public object GetAdditionalPropertyModel(int id)
        {
            var widget = GetWidget(id);
            return GetAdditionalPropertyModel(widget?.WidgetClrType);
        }

        public async Task<Widget> UpdateWidget(UpdateWidgetModel model, object additionalPropertyModel)
        {
            var widget = GetWidget(model.Id);
            _mapper.Map(model, widget);

            if (additionalPropertyModel != null)
                _mapper.Map(additionalPropertyModel, widget);

            await _repository.Update(widget);

            return widget;
        }

        public async Task<Widget> DeleteWidget(int id)
        {
            var widget = GetWidget(id);

            await _repository.Delete(widget);

            return widget;
        }
    }
}