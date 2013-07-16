using MrCMS.ACL;

namespace MrCMS.Models
{
    public interface IMenuItem
    {
        string Text { get; }
        string Url { get; }
        bool CanShow { get; }
    }

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