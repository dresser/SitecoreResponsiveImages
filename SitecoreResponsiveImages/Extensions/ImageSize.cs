using Sitecore.Resources.Media;

namespace SitecoreResponsiveImages.Extensions
{
    /// <summary>
    /// Represents the dimensions of an image.
    /// </summary>
    public class ImageSize
    {
        public int Width { get; set; }
        public int? Height { get; set; }

        public ImageSize()
        {
        }

        public ImageSize(string dimensions)
        {
            var d = dimensions.Split('x');
            Width = int.Parse(d[0]);
            int h;
            if (d.Length > 1 && int.TryParse(d[1], out h)) Height = h;
        }

        public static implicit operator ImageSize(string dimensions)
        {
            return new ImageSize(dimensions);
        }

        public MediaUrlOptions GetMediaUrlOptions(MediaUrlOptions baseMediaUrlOptions = null)
        {
            MediaUrlOptions mediaUrlOptions = baseMediaUrlOptions ?? new MediaUrlOptions();
            mediaUrlOptions.Width = this.Width;
            if (this.Height.HasValue)
            {
                mediaUrlOptions.Height = this.Height.Value;
            }
            return mediaUrlOptions;
        }
    }
}