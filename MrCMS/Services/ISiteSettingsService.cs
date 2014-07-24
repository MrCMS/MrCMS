using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web.Mvc;
using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    //public interface ISiteSettingsService
    //{
    //    SiteSettings GetAllSettings();
    //    void SaveSettings(SiteSettings settings);

    //    T GetSettingValue<T>(string settingName);
    //    void SaveSetting<T>(string settingName, T settingValue);
    //    List<SelectListItem> GetLayoutOptions(int? selectedLayoutId, bool includeDefault = false);
    //}

    //public class SiteSettings
    //{
    //    private Size _thumbnailSize;
    //    private Size _largeSize;
    //    private Size _mediumSize;
    //    private Size _smallSize;
    //    public const string DefaultLayoutKey = "DefaultLayout";
    //    public const string Error404PageIdKey = "Error404PageId";
    //    public const string Error500PageIdKey = "Error500PageId";
    //    public const string MediaDirectoryKey = "MediaDirectory";
    //    public const string ThumbnailDirectoryKey = "ThumbnailDirectory";
    //    public const string ThumbnailImageHeightKey = "ThumbnailImageHeight";
    //    public const string ThumbnailImageWidthKey = "ThumbnailImageWidth";
    //    public const string LargeImageHeightKey = "LargeImageHeight";
    //    public const string LargeImageWidthKey = "LargeImageWidth";
    //    public const string MediumImageHeightKey = "MediumImageHeight";
    //    public const string MediumImageWidthKey = "MediumImageWidth";
    //    public const string SmallImageHeightKey = "SmallImageHeight";
    //    public const string SmallImageWidthKey = "SmallImageWidth";
    //    public const string SystemEmailAddressKey = "SystemEmail";

    //    [DisplayName("Default Layout")]
    //    public int? DefaultLayoutId { get; set; }
    //    public IEnumerable<SelectListItem> LayoutOptions { get; set; }
    //    public IEnumerable<SelectListItem> Error404PageOptions { get; set; }
    //    public IEnumerable<SelectListItem> Error500PageOptions { get; set; }
        
    //    [DisplayName("404 Page")]
    //    public int Error404PageId { get; set; }
    //    [DisplayName("500 Page")]
    //    public int Error500PageId { get; set; }

    //    [DisplayName("Media Directory")]
    //    public string MediaDirectory { get; set; }
    //    [DisplayName("Thumbnail Directory")]
    //    public string ThumbnailDirectory { get; set; }

    //    [DisplayName("Thumbnail Image Width")]
    //    public int ThumbnailImageWidth { get; set; }
    //    [DisplayName("Thumbnail Image Height")]
    //    public int ThumbnailImageHeight { get; set; }
        
    //    public Size ThumbnailSize
    //    {
    //        get
    //        {
    //            if (_thumbnailSize == Size.Empty)
    //            {
    //                _thumbnailSize = new Size(ThumbnailImageWidth, ThumbnailImageHeight);
    //            }
    //            return _thumbnailSize;
    //        }
    //    }

    //    [DisplayName("Large Image Width")]
    //    public int LargeImageWidth { get; set; }
    //    [DisplayName("Large Image Height")]
    //    public int LargeImageHeight { get; set; }

    //    public Size LargeSize
    //    {
    //        get
    //        {
    //            if (_largeSize == Size.Empty)
    //            {
    //                _largeSize = new Size(LargeImageWidth, LargeImageHeight);
    //            }
    //            return _largeSize;
    //        }
    //    }
    //    [DisplayName("Medium Image Width")]
    //    public int MediumImageWidth { get; set; }
    //    [DisplayName("Medium Image Height")]
    //    public int MediumImageHeight { get; set; }

    //    public Size MediumSize
    //    {
    //        get
    //        {
    //            if (_mediumSize == Size.Empty)
    //            {
    //                _mediumSize = new Size(MediumImageWidth, MediumImageHeight);
    //            }
    //            return _mediumSize;
    //        }
    //    }
    //    [DisplayName("Small Image Width")]
    //    public int SmallImageWidth { get; set; }
    //    [DisplayName("Small Image Height")]
    //    public int SmallImageHeight { get; set; }

    //    public Size SmallSize
    //    {
    //        get
    //        {
    //            if (_smallSize == Size.Empty)
    //            {
    //                _smallSize = new Size(SmallImageWidth, SmallImageHeight);
    //            }
    //            return _smallSize;
    //        }
    //    }

    //    public IEnumerable<Size> Sizes
    //    {
    //        get
    //        {
    //            yield return LargeSize;
    //            yield return MediumSize;
    //            yield return SmallSize;
    //            yield return ThumbnailSize;
    //        }
    //    }

    //    [DisplayName("System Email Address")]
    //    public string SystemEmailAddress { get; set; }
    //}
}