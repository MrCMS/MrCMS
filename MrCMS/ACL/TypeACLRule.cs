using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Apps;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Models;

namespace MrCMS.ACL
{
    public class TypeACLRule : ACLRule
    {
        private List<ACLGroup> _rules;
        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string Delete = "Delete";

        public override string DisplayName
        {
            get { return "Webpage"; }
        }

        public IEnumerable<ACLGroup> WebpageRules
        {
            get { return GetRules().Where(group => group.Type.IsSubclassOf(typeof(Webpage))); }
        }

        public IEnumerable<ACLGroup> WidgetRules
        {
            get { return GetRules().Where(group => group.Type.IsSubclassOf(typeof(Widget))); }
        }

        public override List<ACLGroup> GetRules()
        {
            return _rules = _rules ?? GetRuleTypes()
                                                .Select(
                                                    type => new ACLGroup
                                                                {
                                                                    Name = type.Name.BreakUpString(),
                                                                    Type = type,
                                                                    AppName = GetAppName(type),
                                                                    Operations =
                                                                        Operations.Select(s =>
                                                                                          new ACLOperation
                                                                                              {
                                                                                                  Name = s,
                                                                                                  Key = GetKey(s, type.FullName),
                                                                                              }).ToList()
                                                                }).ToList();
        }

        private string GetAppName(Type type)
        {
            if (MrCMSApp.AppWebpages.ContainsKey(type))
                return MrCMSApp.AppWebpages[type];
            if (MrCMSApp.AppWidgets.ContainsKey(type))
                return MrCMSApp.AppWidgets[type];
            return "System";
        }

        private static List<Type> GetRuleTypes()
        {
            return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<SystemEntity>().FindAll(type => type.IsSubclassOf(typeof(Widget)) || type.IsSubclassOf(typeof(Webpage)));
        }

        protected override List<string> GetOperations()
        {
            return new List<string>
                       {
                           Add,
                           Edit,
                           Delete,
                       };
        }
    }
}