using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Utils;
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

        public virtual void OnSaving(ISession session)
        {
            if (Selected && FormProperty.OnlyOneOptionSelectable)
            {
                foreach (var option in FormProperty.Options.Except(this))
                {
                    option.Selected = false;
                    session.Update(option);
                }
            }
            else if (FormProperty.OnlyOneOptionSelectable && !FormProperty.Options.Except(this).Any())
            {
                Selected = true;
            }
        }
    }
}