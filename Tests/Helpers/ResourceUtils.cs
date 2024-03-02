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
    public static class ResourceUtils
    {
        public static readonly string FolderPath = Path.Combine("src", "imagedb");


        public static FileInfo[] GetRandomly(Random random, params string[] mimes)
        {
            
            
            var allImages = Directory.GetFiles(FolderPath)
                .Where(f => mimes.Length == 0 || mimes.Contains(MimeUtility.GetMimeMapping(f)))
                .Select(im => new FileInfo(im))
                .ToArray();

            random.Shuffle(allImages);

            return allImages;

        }
    }
}
