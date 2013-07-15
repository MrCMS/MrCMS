using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;

namespace MrCMS.ACL
{
    public class WidgetACL : ACLRule
    {
        public override string DisplayName
        {
            get { return "Widget"; }
        }

        public override string GetKey(string operation, IDictionary<string, string> customData)
        {
            var widgetType = string.Empty;
            if (customData != null)
                customData.TryGetValue("widget-type", out widgetType);
            return !string.IsNullOrWhiteSpace(operation)
                       ? string.Format("{0}.{1}.{2}", Name, widgetType, operation)
                       : string.Format("{0}.{1}", Name, widgetType);
        }

        public override IDictionary<string, List<ACLOperation>> GetRules()
        {
            return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Widget>()
                              .ToDictionary(type => type.FullName, type => Operations.Select(s => new ACLOperation
                              {
                                  Name = s,
                                  OperationKey = GetKey(s, new Dictionary<string, string> { { "widget-type", type.FullName } })
                              }).ToList());
        }

        protected override List<string> GetOperations()
        {
            return new List<string>
                       {
                           "Add",
                           "Edit",
                           "Delete",
                       };
        }
    }
}