using Bogus;
using MimeMapping;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Helpers
{
    public static class ImageUtils
    {
        public static readonly string FolderPath = Path.Combine("src", "imagesdb");


        public static FileInfo[] GetRandonly(Random random)
        {
            
            
            var allImages = Directory.GetFiles(FolderPath)
                .Where(f => Values.File.SupportedImagesMime.Contains(MimeUtility.GetMimeMapping(f)))
                .Select(im => new FileInfo(im))
                .ToArray();

            random.Shuffle(allImages);

            return allImages;

        }
    }
}
