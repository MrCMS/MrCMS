using System;

namespace MrCMS.Website
{
    public class EndRequestExecutionPriorityAttribute : Attribute
    {
        public int Priority { get; private set; }

        public EndRequestExecutionPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}