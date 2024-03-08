using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public static class StringExtensions
    {
        /// <summary>
        /// Substitui substrings numa string sequencialmente com base em tudo que fica entre chaves
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Placeholder(this string str, params string[] args)
        {
            var pattern = @"\{([^}]*)\}";
            var count = 0;
            var formated = Regex.Replace(str, pattern, match =>
            {
                return "{" + count++ + "}";
            });
            return string.Format(formated, args);
        }

        public static string ToQuery(this NameValueCollection nameValue){

            var sb = new StringBuilder();

            foreach (var key in nameValue.AllKeys)
            {
                sb.Append($"{key}={nameValue[key]}&");
            }

            return sb.ToString()[..(sb.Length - 1)];
        }

        public static string ToQueryWithPrefix(this NameValueCollection nameValue)
            => "?" + nameValue.ToQuery();
    }
}
