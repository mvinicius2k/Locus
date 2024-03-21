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
        public static class Events
        {
            public static int QueryDecode = 1;
            public static int QueryConvert = 2;
        }

        public static class Entity
        {
            public const int GroupNameMaxLength = 64;
            public const int GroupDescriptionMaxLength = 1024;

            public const int PostContentMaxLength = 30000;
            public const int PostSummaryMaxLength = 500;
            public const int PostTitleMaxLength = 70;

            public const int TagNameMaxLength = 32;

            public const int UserPresentationNameMaxLength = 128;

            public const int ResourcePathMaxLength = 256;
            
            

        }

        public static class Api
        {
            public const string FreeImageHostUrl = $"https://freeimage.host/api/1/upload?key={{key}}";
            public const string TagGet = $"Tags-Get";
            public const string TagGetById = $"Tags-GetById";
            public const string TagAdd = $"Tags-Add";
            public const string TagRename = $"Tags-Rename";
            public const string TagDelete = $"Tags-Delete";
            public const string TagGetQuery = "query";
            //public const string TagFunctionsGetQuery = $"?query={{query}}";
        }

        public static class File
        {
            [Obsolete]
            public static readonly string[] SupportedImagesMime = new string[]
            {
                KnownMimeTypes.Jpg, KnownMimeTypes.Png, KnownMimeTypes.Bmp, KnownMimeTypes.Avif, KnownMimeTypes.Tif, KnownMimeTypes.Gif, KnownMimeTypes.Webp
            };
        }
    }
}
