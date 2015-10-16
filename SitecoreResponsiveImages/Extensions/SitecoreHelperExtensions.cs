using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Mvc.Helpers;
using Sitecore.Resources.Media;
using SitecoreResponsiveImages.Util;
using System.Linq;
using System.Text;
using System.Web;

namespace SitecoreResponsiveImages.Extensions
{
    public static class SitecoreHelperExtensions
    {
        private static MediaItem GetMediaItem(string fieldName, Item item)
        {
            if (item == null || item.Fields[fieldName] == null)
            {
                return null;
            }
            var imageField = new ImageField(item.Fields[fieldName]);
            if (imageField.MediaItem == null)
            {
                return null;
            }
            return imageField.MediaItem;
        }        
        
        private static string GetUrl(string fieldName, Item item, object parameters)
        {
            var mediaUrlOptions = MediaUrlOptionsHelper.ParseMediaUrlOptions(parameters);
            return GetUrl(fieldName, item, mediaUrlOptions);
        }

        private static string GetUrl(string fieldName, Item item, MediaUrlOptions mediaUrlOptions)
        {
            var mediaItem = GetMediaItem(fieldName, item);
            return GetUrl(mediaItem, mediaUrlOptions);
        }

        private static string GetUrl(MediaItem mediaItem, MediaUrlOptions mediaUrlOptions)
        {
            if (mediaItem == null)
            {
                return null;
            }
            return HashingUtils.ProtectAssetUrl(MediaManager.GetMediaUrl(mediaItem, mediaUrlOptions));
        }

        /// <summary>
        /// Returns a string containing the hash-protected image URL.
        /// </summary>
        /// <param name="sitecoreHelper"></param>
        /// <param name="fieldName"></param>
        /// <param name="item"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string ImageSrc(this SitecoreHelper sitecoreHelper, string fieldName, Item item = null, object parameters = null)
        {
            if (item == null)
            {
                item = sitecoreHelper.CurrentItem;
            }
            return GetUrl(fieldName, item, parameters);
        }

        public static string ImageAlt(this SitecoreHelper sitecoreHelper, string fieldName, Item item = null)
        {
            if (item == null)
            {
                item = sitecoreHelper.CurrentItem;
            }
            var mediaItem = GetMediaItem(fieldName, item);
            if (mediaItem == null)
            {
                return null;
            }
            return mediaItem.Alt;
        }

        /// <summary>
        /// Returns a string containing a comma separated list of the different urls.
        /// </summary>
        /// <param name="imageSizes"></param>
        /// <param name="baseMediaUrlOptions"></param>
        /// <returns></returns>
        private static string GetUrlsString(MediaItem mediaItem, ImageSizeList imageSizes, MediaUrlOptions baseMediaUrlOptions = null)
        {
            return string.Join(", ", imageSizes.Select(imgSize => GetUrl(mediaItem, imgSize.GetMediaUrlOptions(baseMediaUrlOptions))));
        }

        public static HtmlString ResponsiveImage(this SitecoreHelper sitecoreHelper, string fieldName, object parameters = null, ResponsiveSizes responsiveSizes = null)
        {
            return ResponsiveImage(sitecoreHelper, fieldName, (Item)null, parameters, responsiveSizes);
        }
        
        public static HtmlString ResponsiveImage(this SitecoreHelper sitecoreHelper, string fieldName, Item item = null, object parameters = null, ResponsiveSizes responsiveSizes = null)
        {
            if (item == null)
            {
                item = sitecoreHelper.CurrentItem;
            }
            if (responsiveSizes == null || Sitecore.Context.PageMode.IsPageEditorEditing)
            {
                return sitecoreHelper.Field(fieldName, item, parameters);
            }
            var baseMediaUrlOptions = MediaUrlOptionsHelper.ParseMediaUrlOptions(parameters);
            var mediaItem = GetMediaItem(fieldName, item);
            var src = GetUrl(mediaItem, baseMediaUrlOptions);
            if (responsiveSizes.Default != null)
            {
                src = GetUrl(mediaItem, responsiveSizes.Default.GetMediaUrlOptions(baseMediaUrlOptions));
            }
            var properties = MediaUrlOptionsHelper.ParseUnusedParameters(parameters).Aggregate(string.Empty, (current, s) => current + string.Format("{0}=\"{1}\" ", s.Key, s.Value));
            var html = new StringBuilder("<img " + properties);
            html.AppendFormat(" src=\"{0}\"", src);
            html.AppendFormat(" alt=\"{0}\"", mediaItem.Alt);

            if (responsiveSizes.Mobile != null)
            {
                html.AppendFormat(" data-responsimg-mobile=\"{0}\"", GetUrlsString(mediaItem, responsiveSizes.Mobile, baseMediaUrlOptions));
            }
            if (responsiveSizes.Tablet != null)
            {
                html.AppendFormat(" data-responsimg-tablet=\"{0}\"", GetUrlsString(mediaItem, responsiveSizes.Tablet, baseMediaUrlOptions));
            }
            if (responsiveSizes.Desktop != null)
            {
                html.AppendFormat(" data-responsimg-desktop=\"{0}\"", GetUrlsString(mediaItem, responsiveSizes.Desktop, baseMediaUrlOptions));
            }

            html.Append(" />");
            return new HtmlString(html.ToString());
        }
    }
}