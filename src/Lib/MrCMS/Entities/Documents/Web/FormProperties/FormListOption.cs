using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;

namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public class FormListOption : SiteEntity
    {
        public virtual FormPropertyWithOptions FormProperty { get; set; }
        [Required]
        public virtual string Value { get; set; }
        public virtual bool Selected { get; set; }

        public virtual async Task OnSaving(IRepository<FormListOption> repository)
        {
            var otherOptions = FormProperty.Options.Except(new[] { this }).ToList();
            if (Selected && FormProperty.OnlyOneOptionSelectable)
            {
                foreach (var option in otherOptions)
                {
                    option.Selected = false;
                }
                await repository.UpdateRange(otherOptions);
            }
            else if (FormProperty.OnlyOneOptionSelectable && !otherOptions.Any())
            {
                Selected = true;
            }
        }
    }
}