using System;

namespace MrCMS.Entities
{
    public interface IHaveSystemDates
    {
        DateTime CreatedOn { get; set; }
        DateTime UpdatedOn { get; set; }
    }
}