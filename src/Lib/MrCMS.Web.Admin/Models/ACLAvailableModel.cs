namespace MrCMS.Web.Admin.Models
{
    public class ACLAvailableModel
    {
        //public static ACLAvailableModel Create(ACLRoleModel role, ACLRule rule, string operation, Type type)
        //{
        //    return new ACLAvailableModel
        //               {
        //                   Role = role,
        //                   IsAllowed = rule.CanAccess(role.Role, operation, type == null ? null : type.FullName)
        //               };
        //}

        //private ACLAvailableModel()
        //{

        //}
        public ACLRoleModel Role { get; set; }
        public bool IsAllowed { get; set; }

        public string RoleName
        {
            get { return Role != null ? Role.Name : string.Empty; }
        }
    }
}