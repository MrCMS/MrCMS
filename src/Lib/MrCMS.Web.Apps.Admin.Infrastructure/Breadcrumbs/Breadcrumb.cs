using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{

    // ReSharper disable once UnusedTypeParameter -- used to establish hierarchy when read via reflection
    public abstract class Breadcrumb<T> : Breadcrumb where T : Breadcrumb
    {
    }
    public abstract class Breadcrumb
    {
        public virtual string Title => Name;
        // default to type name with Breadcrumb removed

        protected Breadcrumb()
        {
            Name = GetType().Name.Replace("Breadcrumb", "").BreakUpString();
        }

        public virtual string Name { get; protected set; }

        public virtual int Order => 0;

        public virtual bool IsNav => false; 

        public abstract string Controller { get; }
        public abstract string Action { get; }
        public virtual int? Id { get; set; }

        public virtual string Url(IUrlHelper url) => IsPlaceHolder ? null : url.Action(Action, Controller, new {Id});

        /// <summary>
        /// If this is true, it will use the parent id to get another of the same type rather than go to the parent type 
        /// </summary>
        public virtual bool Hierarchical { get; }
        public virtual string CssClass { get; }

        /// <summary>
        /// Used to bypass check for descriptor/ACL as there isn't a corresponding route
        /// </summary>
        public virtual bool IsPlaceHolder { get; }


        public virtual int? ParentId { get; protected set; }
        public virtual bool ShouldSkip { get; }

        /// <summary>
        /// This is to be called once to initialize the breadcrumb with data.
        /// Should be used for getting the data for parent ids and names
        /// </summary>
        public virtual void Populate()
        {
        }
    }
}