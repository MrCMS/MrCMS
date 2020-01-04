using Microsoft.EntityFrameworkCore;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Entities.Widget;

namespace MrCMS.DbConfiguration
{
    //public class CreateCoreModel : ICreateModel
    //{
    //    public const string WebpageDiscriminator = "WebpageType";
    //    public const string WidgetDiscriminator = "WidgetType";
    //    public const string MenuItemDiscriminator = "MenuItemType";
    //    private readonly IReflectionHelper _reflectionHelper;

    //    public CreateCoreModel(IReflectionHelper reflectionHelper)
    //    {
    //        _reflectionHelper = reflectionHelper;
    //    }

    //    public void Create(ModelBuilder builder)
    //    {
    //        builder.Entity<Site>();

    //        builder.Entity<Setting>();
    //        builder.Entity<SystemSetting>();

    //        builder.Entity<Webpage>(typeBuilder =>
    //        {
    //            typeBuilder.HasIndex(x => new { x.SiteId, x.UrlSegment }).IsUnique();
    //            typeBuilder.HasOne(x => x.Site).WithMany().OnDelete(DeleteBehavior.Restrict);

    //            var webpageBuilder = typeBuilder.HasDiscriminator<string>(WebpageDiscriminator);
    //            foreach (var type in _reflectionHelper.GetAllConcreteImplementationsOf(typeof(Webpage)))
    //                webpageBuilder.HasValue(type, type.Name);
    //        });

    //        builder.Entity<UrlHistory>(typeBuilder =>
    //        {
    //            typeBuilder.HasIndex(x => new { x.SiteId, x.UrlSegment }).IsUnique();
    //            typeBuilder.HasOne(x => x.Site).WithMany().OnDelete(DeleteBehavior.Restrict);
    //        });

    //        builder.Entity<Layout>();

    //        builder.Entity<LayoutArea>();

    //        builder.Entity<FileFolder>();

    //        builder.Entity<File>(typeBuilder =>
    //        {
    //            typeBuilder.HasIndex(file => new { file.Slug, file.Extension }).IsUnique();
    //            typeBuilder.Ignore(x => x.Name);
    //        });

    //        builder.Entity<ImageSizeConfiguration>(typeBuilder =>
    //        {
    //            typeBuilder.HasIndex(file => new { file.Name }).IsUnique();
    //        });

    //        var widgetBuilder = builder.Entity<Widget>().HasDiscriminator<string>(WidgetDiscriminator);
    //        foreach (var type in _reflectionHelper.GetAllConcreteImplementationsOf(typeof(Widget)))
    //            widgetBuilder.HasValue(type, type.Name);

    //        builder.Entity<HeaderNavigation>(nav =>
    //        {
    //            nav.HasOne(x => x.Menu).WithMany().OnDelete(DeleteBehavior.Restrict);
    //        });

    //        builder.Entity<Localization>(
    //            r => r.HasAlternateKey(c => new { c.Key, c.Culture }));

    //        builder.Entity<Menu>(menu =>
    //        {
    //            menu.HasIndex(m => new { m.SiteId, m.Name }).IsUnique();
    //        });

    //        var menuItemBuilder = builder.Entity<MenuItem>().HasDiscriminator<string>(MenuItemDiscriminator);
    //        foreach (var type in _reflectionHelper.GetAllConcreteImplementationsOf(typeof(MenuItem)))
    //            menuItemBuilder.HasValue(type, type.Name);

    //        builder.Entity<Gallery>();
    //        builder.Entity<GalleryItem>(gi =>
    //        {
    //            gi.HasKey(item => new {item.GalleryId, item.FileId});
    //            gi.HasOne(x => x.Gallery).WithMany().OnDelete(DeleteBehavior.Restrict);
    //            gi.HasOne(x => x.File).WithMany().OnDelete(DeleteBehavior.Restrict);
    //        });

    //        builder.Entity<EmailTemplateView>();
    //    }
    //}
}