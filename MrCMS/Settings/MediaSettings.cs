using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Website.Caching;
using NHibernate;

namespace MrCMS.Settings
{
    public class MediaSettings : SiteSettingsBase
    {
        private readonly SiteSettingsOptionGenerator _siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();
        private Size _largeSize;
        private Size _maxSize;
        private Size _mediumSize;
        private Size _smallSize;
        private Size _thumbnailSize;

        public MediaSettings()
        {
            MaxFileSizeUpload = 5000000; //5mb
            AllowedFileTypes = "gif|jpeg|jpg|png|rar|zip|pdf|mp3|mp4|wmv|doc|docx|xls|xlsx|ppt|pptx|avi|mpg|wav|mov|wma|webm|ogv|mpeg|flv|7z|txt|csv|html|htm";
            EnforceMaxImageSize = true;
            MaxImageSizeHeight = 1200;
            MaxImageSizeWidth = 1200;
        }

        [DisplayName("Thumbnail Image Height")]
        public int ThumbnailImageHeight { get; set; }

        [DisplayName("Thumbnail Image Width")]
        public int ThumbnailImageWidth { get; set; }

        [DisplayName("Large Image Height")]
        public int LargeImageHeight { get; set; }

        [DisplayName("Large Image Width")]
        public int LargeImageWidth { get; set; }

        [DisplayName("Medium Image Height")]
        public int MediumImageHeight { get; set; }

        [DisplayName("Medium Image Width")]
        public int MediumImageWidth { get; set; }

        [DisplayName("Small Image Height")]
        public int SmallImageHeight { get; set; }

        [DisplayName("Small Image Width")]
        public int SmallImageWidth { get; set; }

        public Size ThumbnailSize
        {
            get
            {
                if (_thumbnailSize == Size.Empty)
                {
                    _thumbnailSize = new Size(ThumbnailImageWidth, ThumbnailImageHeight);
                }
                return _thumbnailSize;
            }
        }

        public Size LargeSize
        {
            get
            {
                if (_largeSize == Size.Empty)
                {
                    _largeSize = new Size(LargeImageWidth, LargeImageHeight);
                }
                return _largeSize;
            }
        }

        public Size MediumSize
        {
            get
            {
                if (_mediumSize == Size.Empty)
                {
                    _mediumSize = new Size(MediumImageWidth, MediumImageHeight);
                }
                return _mediumSize;
            }
        }

        public Size SmallSize
        {
            get
            {
                if (_smallSize == Size.Empty)
                {
                    _smallSize = new Size(SmallImageWidth, SmallImageHeight);
                }
                return _smallSize;
            }
        }

        public IEnumerable<Size> Sizes
        {
            get
            {
                yield return LargeSize;
                yield return MediumSize;
                yield return SmallSize;
                yield return ThumbnailSize;
            }
        }

        public IEnumerable<ImageSize> ImageSizes
        {
            get
            {
                if (LargeSize != Size.Empty)
                    yield return new ImageSize("Large", LargeSize);
                if (MediumSize != Size.Empty)
                    yield return new ImageSize("Medium", MediumSize);
                if (SmallSize != Size.Empty)
                    yield return new ImageSize("Small", SmallSize);
                if (ThumbnailSize != Size.Empty)
                    yield return new ImageSize("Thumbnail", ThumbnailSize);
            }
        }


        [DisplayName("Enforce Max Image Size")]
        public bool EnforceMaxImageSize { get; set; }

        [DisplayName("Max Image Size Height")]
        public int MaxImageSizeHeight { get; set; }

        [DisplayName("Max Image Size Width")]
        public int MaxImageSizeWidth { get; set; }

        [DisplayName("Admin allowed file type uploads")]
        public string AllowedFileTypes { get; set; }

        public IEnumerable<string> AllowedFileTypeList
        {
            get { return AllowedFileTypes.Split('|'); }
        }

        [DisplayName("Admin max file upload size (Max 50000000 (500 mb))")]
        public int MaxFileSizeUpload { get; set; }

        public Size MaxSize
        {
            get
            {
                if (_maxSize == Size.Empty)
                {
                    _maxSize = new Size(MaxImageSizeHeight, MaxImageSizeWidth);
                }
                return _maxSize;
            }
        }

        [DisplayName("Cache image rendering?")]
        public virtual bool Cache { get; set; }
        [DisplayName("Cache for how many seconds?")]
        public virtual int CacheLength { get; set; }
        [DisplayName("Cache expiry type")]
        [DropDownSelection("CacheExpiryTypeOptions")]
        public CacheExpiryType CacheExpiryType { get; set; }

        public int? ResizeQuality { get; set; }

        public override bool RenderInSettings
        {
            get { return true; }
        }

        public override void SetViewData(ISession session, ViewDataDictionary viewDataDictionary)
        {
            viewDataDictionary["DefaultCategoryOptions"] = _siteSettingsOptionGenerator.GetMediaCategoryOptions(session, null);
            viewDataDictionary["CacheExpiryTypeOptions"] = _siteSettingsOptionGenerator.GetCacheExpiryTypeOptions();
        }
    }
}