using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.ACL
{
    public class WebpageACL : ACLRule
    {
        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string Delete = "Delete";

        public override string DisplayName
        {
            get { return "Webpage"; }
        }

        public override string GetKey(string operation, IDictionary<string, string> customData)
        {
            var pageType = string.Empty;
            if (customData != null)
                customData.TryGetValue("page-type", out pageType);
            return !string.IsNullOrWhiteSpace(operation)
                       ? string.Format("{0}.{1}.{2}", Name, pageType, operation)
                       : string.Format("{0}.{1}", Name, pageType);
        }

        public override IDictionary<string, List<ACLOperation>> GetRules()
        {
            return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                             .ToDictionary(type => type.FullName,
                                           type => Operations.Select(s =>
                                                                     new ACLOperation
                                                                         {
                                                                             Name = s,
                                                                             OperationKey = GetKey(s, new Dictionary<string, string> { { "page-type", type.FullName } })
                                                                         }).ToList());
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