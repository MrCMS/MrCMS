using MrCMS.ACL;

namespace MrCMS.Models
{
    public class ACLOption
    {
        public ACLRule Rule { get; set; }
        public string Operation { get; set; }

        public static ACLOption Create(ACLRule rule, string operation)
        {
            return new ACLOption {Operation = operation, Rule = rule};
        }
    }
}