using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{

    public static partial class Values
    {
        public static partial class Entity
        {
            public const int GroupNameMaxLength = 64;
            public const int GroupDescriptionMaxLength = 1024;

            public const int PostContentMaxLength = 2 ^ 16 - 1;
            public const int PostTitleMaxLength = 70;

            public const int TagNameMaxLength = 32;

            public const int UserPresentationNameMaxLength = 128;

            public const int ResourcePathMaxLength = 256;
            


        }

        public static partial class Api
        {
            public static readonly Uri RequestFreeImageHostUrl = new Uri("https://freeimage.host/api/1/upload");
        }
    }
}
