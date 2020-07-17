using System;
using System.Linq;
using AutoMapper;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Infrastructure.ModelBinding;
using NHibernate;

namespace MrCMS.Web.Admin.Services
{
    public class WidgetAdminService : IWidgetAdminService
    {
        private readonly ISession _session;
        private readonly IMapper _mapper;

        public WidgetAdminService(ISession session, IMapper mapper)
        {
            _session = session;
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

        public Widget AddWidget(AddWidgetModel model, object additionalPropertyModel)
        {
            var type = TypeHelper.GetTypeByName(model.WidgetType);
            var instance = Activator.CreateInstance(type) as Widget;
            _mapper.Map(model, instance);
            if (additionalPropertyModel != null)
                _mapper.Map(additionalPropertyModel, instance);

            instance.LayoutArea.AddWidget(instance);
            _session.Transact(session => session.Save(instance));

            return instance;
        }

        public UpdateWidgetModel GetEditModel(int id)
        {
            return _mapper.Map<UpdateWidgetModel>(GetWidget(id));
        }

        public Widget GetWidget(int id)
        {
            return _session.Get<Widget>(id);
        }

        public object GetAdditionalPropertyModel(int id)
        {
            var widget = GetWidget(id);
            return GetAdditionalPropertyModel(widget?.WidgetType);
        }

        public Widget UpdateWidget(UpdateWidgetModel model, object additionalPropertyModel)
        {
            var widget = GetWidget(model.Id);
            _mapper.Map(model, widget);

            if (additionalPropertyModel != null)
                _mapper.Map(additionalPropertyModel, widget);

            _session.Transact(session => session.Update(widget));

            return widget;
        }

        public Widget DeleteWidget(int id)
        {
            var widget = GetWidget(id);

            _session.Transact(session => session.Delete(widget));

            return widget;
        }
    }
}