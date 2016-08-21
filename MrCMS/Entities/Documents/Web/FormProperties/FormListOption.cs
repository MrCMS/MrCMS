using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Utils;
using MrCMS.Data;
using NHibernate;
using NHibernate.Util;

namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public class FormListOption : SiteEntity
    {
        public virtual FormPropertyWithOptions FormProperty { get; set; }
        [Required]
        public virtual string Value { get; set; }
        public virtual bool Selected { get; set; }

        public virtual void OnSaving(IRepository<FormListOption> repository)
        {
            if (Selected && FormProperty.OnlyOneOptionSelectable)
            {
                foreach (var option in FormProperty.Options.Except(this))
                {
                    option.Selected = false;
                    repository.Update(option);
                }
            }
            else if (FormProperty.OnlyOneOptionSelectable && !FormProperty.Options.Except(this).Any())
            {
                Selected = true;
            }
        }
    }
}