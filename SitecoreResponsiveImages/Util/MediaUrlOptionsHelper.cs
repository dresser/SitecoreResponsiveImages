using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Collections;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.Mvc.Helpers;
using Sitecore;

namespace SitecoreResponsiveImages.Util
{
    public class MediaUrlOptionsHelper
    {
        public class Params
        {
            public const string AllowStretch = "as";
            public const string IgnoreAspectRatio = "iar";
            public const string Width = "w";
            public const string Height = "h";
            public const string Scale = "sc";
            public const string MaxWidth = "mw";
            public const string MaxHeight = "mh";
            public const string Thumbnail = "thn";
            public const string BackgroundColor = "bc";
            public const string Database = "db";
            public const string Language = "la";
            public const string Version = "vs";
            public const string DisableMediaCache = "disableMediaCache";
            public const string DisableWebEdit = "DisableWebEdit";
            public static ReadOnlyStringList All = new ReadOnlyStringList(new[] { AllowStretch, IgnoreAspectRatio, Width, Height, Scale, MaxWidth, MaxHeight, Thumbnail, BackgroundColor, Database, Language, Version, DisableMediaCache, DisableWebEdit });
        }

        /// <summary>
        /// 'Inspired by' Sitecore.Xml.Xsl.ImageRenderer.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static MediaUrlOptions ParseMediaUrlOptions(object parameters)
        {
            var mediaUrlOptions = new MediaUrlOptions();
            var safeParameters = new SafeDictionary<string>();
            if (parameters != null)
            {
                TypeHelper.CopyProperties(parameters, safeParameters);
            }
            var valueSet = false;
            var allowStretch = MainUtil.GetBool(Extract(safeParameters, ref valueSet, Params.AllowStretch), false);
            if (valueSet)
            {
                mediaUrlOptions.AllowStretch = allowStretch;
            }
            var ignoreAspectRatio = MainUtil.GetBool(Extract(safeParameters, ref valueSet, Params.IgnoreAspectRatio), false);
            if (valueSet)
            {
                mediaUrlOptions.IgnoreAspectRatio = ignoreAspectRatio;
            }
            var width = MainUtil.GetInt(Extract(safeParameters, ref valueSet, Params.Width), 0);
            if (valueSet)
            {
                mediaUrlOptions.Width = width;
            }
            var height = MainUtil.GetInt(Extract(safeParameters, ref valueSet, Params.Height), 0);
            if (valueSet)
            {
                mediaUrlOptions.Height = height;
            }
            var scale = MainUtil.GetFloat(Extract(safeParameters, ref valueSet, Params.Scale), 0.0f);
            if (valueSet)
            {
                mediaUrlOptions.Scale = scale;
            }
            var maxWidth = MainUtil.GetInt(Extract(safeParameters, ref valueSet, Params.MaxWidth), 0);
            if (valueSet)
            {
                mediaUrlOptions.MaxWidth = maxWidth;
            }
            var maxHeight = MainUtil.GetInt(Extract(safeParameters, ref valueSet, Params.MaxHeight), 0);
            if (valueSet)
            {
                mediaUrlOptions.MaxHeight = maxHeight;
            }
            var thumbnail = MainUtil.GetBool(Extract(safeParameters, ref valueSet, Params.Thumbnail), false);
            if (valueSet)
            {
                mediaUrlOptions.Thumbnail = thumbnail;
            }
            var backgroundColor = Extract(safeParameters, ref valueSet, Params.BackgroundColor) ?? string.Empty;
            if (valueSet)
            {
                mediaUrlOptions.BackgroundColor = MainUtil.StringToColor(backgroundColor);
            }
            var database = Extract(safeParameters, ref valueSet, Params.Database);
            if (valueSet)
            {
                mediaUrlOptions.Database = Sitecore.Configuration.Factory.GetDatabase(database);
            }
            var language = Extract(safeParameters, ref valueSet, Params.Language);
            Sitecore.Globalization.Language parsedlanguage;
            if (valueSet && Sitecore.Globalization.Language.TryParse(language, out parsedlanguage))
            {
                mediaUrlOptions.Language = parsedlanguage;
            }
            var version = Extract(safeParameters, ref valueSet, Params.Version);
            Sitecore.Data.Version parsedVersion;
            if (valueSet && Sitecore.Data.Version.TryParse(version, out parsedVersion))
            {
                mediaUrlOptions.Version = parsedVersion;
            }
            var disableMediaCache = MainUtil.GetBool(Extract(safeParameters, ref valueSet, Params.DisableMediaCache), false);
            if (valueSet)
            {
                mediaUrlOptions.DisableMediaCache = disableMediaCache;
            }
            return mediaUrlOptions;
        }

        /// <summary>
        /// Returns a SafeDictionary[string] of the parameters <b>not</b> needed by MediaUrlOptions.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static SafeDictionary<string> ParseUnusedParameters(object parameters)
        {
            var safeParameters = new SafeDictionary<string>();
            var unusedParameters = new SafeDictionary<string>();
            if (parameters != null)
            {
                TypeHelper.CopyProperties(parameters, safeParameters);
                foreach (var param in safeParameters)
                {
                    if (!Params.All.Contains(param.Key))
                    {
                        unusedParameters.Add(param.Key, param.Value);
                    }
                }
            }
            return unusedParameters;
        }

        public static SafeDictionary<string> ParseUnusedParameters(object parameters, params string[] additionalExcludedParameters)
        {
            var unusedParameters = ParseUnusedParameters(parameters);
            if (additionalExcludedParameters != null && additionalExcludedParameters.Any())
            {
                foreach (var additionalParameter in additionalExcludedParameters)
                {
                    unusedParameters.Remove(additionalParameter);
                }
            }
            return unusedParameters;
        }

        /// <summary>
        /// 'Borrowed' from Sitecore.Xml.Xsl.ImageRenderer
        /// </summary>
        /// <param name="values"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        private static string Extract(SafeDictionary<string> values, params string[] keys)
        {
            Assert.ArgumentNotNull((object)values, "values");
            Assert.ArgumentNotNull((object)keys, "keys");
            foreach (string key in keys)
            {
                string str = values[key];
                if (str != null)
                {
                    values.Remove(key);
                    return str;
                }
            }
            return (string)null;
        }

        /// <summary>
        /// 'Borrowed' from Sitecore.Xml.Xsl.ImageRenderer
        /// </summary>
        /// <param name="values"></param>
        /// <param name="valueSet"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        private static string Extract(SafeDictionary<string> values, ref bool valueSet, params string[] keys)
        {
            Assert.ArgumentNotNull((object)values, "values");
            Assert.ArgumentNotNull((object)keys, "keys");
            string str = Extract(values, keys);
            valueSet = str != null;
            return str;
        }
    }
}
