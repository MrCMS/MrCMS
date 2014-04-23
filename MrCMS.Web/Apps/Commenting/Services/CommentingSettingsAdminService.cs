using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Commenting.Settings;
using System.Linq;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public class CommentingSettingsAdminService : ICommentingSettingsAdminService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IProcessCommentWidgetChanges _processCommentWidgetChanges;

        public CommentingSettingsAdminService(IConfigurationProvider configurationProvider, IProcessCommentWidgetChanges processCommentWidgetChanges)
        {
            _configurationProvider = configurationProvider;
            _processCommentWidgetChanges = processCommentWidgetChanges;
        }

        public List<Type> GetAllPageTypes()
        {
            return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>();
        }

        public void UpdateSettings(CommentingSettings settings)
        {
            var previousSettings = _configurationProvider.GetSiteSettings<CommentingSettings>();
            _configurationProvider.SaveSettings(settings);
            _processCommentWidgetChanges.Process(previousSettings, settings);
        }

        public CommentingSettings GetSettings()
        {
            return _configurationProvider.GetSiteSettings<CommentingSettings>();
        }

        public List<SelectListItem> GetCommentApprovalTypes()
        {
            return
                Enum.GetValues(typeof (CommentApprovalType))
                    .Cast<CommentApprovalType>()
                    .BuildSelectItemList(type => type.ToString(), emptyItem: null);
        }
    }
}