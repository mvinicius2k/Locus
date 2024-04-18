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
        /// Converte para uma string de tamanho 22 base64 segura para urls, sem {/, +, =}
        /// </summary>
        public static string ToSafeBase64(this Guid guid, char plusReplacer = '_', char barReplacer = ',')
        {
            return Convert.ToBase64String(guid.ToByteArray()).Substring(0, 22)
                .Replace('+', plusReplacer)
                .Replace('/', barReplacer)
                .Replace("=", "");
        }

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

        /// <summary>
        /// Cria uma query de url com os itens dividos por '&amp;' , iniciando com o prefixo '?'
        /// </summary>
        /// <param name="nameValue"></param>
        /// <returns></returns>
        public static string ToQueryWithPrefix(this NameValueCollection nameValue)
            => "?" + nameValue.ToQuery();
    }
}
