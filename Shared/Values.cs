using MimeMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{

    public static class Values
    {
        public static class Entity
        {
            public const int GroupNameMaxLength = 64;
            public const int GroupDescriptionMaxLength = 1024;

            public const int PostContentMaxLength = 2 ^ 16 - 1;
            public const int PostTitleMaxLength = 70;

            public const int TagNameMaxLength = 32;

            public const int UserPresentationNameMaxLength = 128;

            public const int ResourcePathMaxLength = 256;
            


        }

        public static class Api
        {
            public const string RequestFreeImageHostRoute = $"https://freeimage.host/api/1/upload?key={{key}}";
        }

        public static class File
        {
            public static readonly string[] SupportedImagesMime = new string[]
            {
                KnownMimeTypes.Jpg, KnownMimeTypes.Png, KnownMimeTypes.Bmp, KnownMimeTypes.Avif, KnownMimeTypes.Tif, KnownMimeTypes.Gif, KnownMimeTypes.Webp
            };
        }
    }
}
