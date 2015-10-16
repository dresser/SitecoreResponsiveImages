using System.Collections.Generic;
using System.Linq;

namespace SitecoreResponsiveImages.Extensions
{
    public class ImageSizeList : List<ImageSize>
    {
        public ImageSizeList() : base() { }

        public ImageSizeList(IEnumerable<ImageSize> imageSizes) : base(imageSizes) { }

        public static implicit operator ImageSizeList(string dimensionList)
        {
            if (string.IsNullOrWhiteSpace(dimensionList))
            {
                return new ImageSizeList();
            }
            var imageSizes = dimensionList.Split(new char[] { ',' });
            return new ImageSizeList(imageSizes.Select(img => new ImageSize(img)));
        }
    }
}