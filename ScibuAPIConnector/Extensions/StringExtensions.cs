using System;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace ScibuAPIConnector.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceNumbers(string text) =>
            Regex.Replace(Regex.Replace(text, @"\d{2,}", ""), @"\d", "");

        public static string Left(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength)
                   );
        }

        public static string GetNumbers(string input) =>
        new string((from c in input
                    where char.IsDigit(c)
                    select c).ToArray<char>());


        public static string HtmlDecode(this string str) =>
            WebUtility.HtmlDecode(str);

        public static string Right(this string sValue, int iMaxLength)
        {
            //Check if the value is valid
            if (string.IsNullOrEmpty(sValue))
            {
                //Set valid empty string as string could be null
                sValue = string.Empty;
            }
            else if (sValue.Length > iMaxLength)
            {
                //Make the string no longer than the max length
                sValue = sValue.Substring(sValue.Length - iMaxLength, iMaxLength);
            }

            //Return the string
            return sValue;
        }

        public static bool HasContent(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char ch in str)
            {
                if ((((ch >= '0') && (ch <= '9')) || (((ch >= 'A') && (ch <= 'Z')) || (((ch >= 'a') && (ch <= 'z')) || ((ch == '.') || ((ch == '_') || ((ch == '-') || ((ch == ' ') || ((ch == ',') || ((ch == '@') || ((ch == '&') || ((ch == ';') || ((ch == '+') || ((ch == ':') || ((ch == '/') || ((ch == '\\') || ((ch == '(') || ((ch == ')') || ((ch == '\'') || ((ch == '\'') || ((ch == '—') || ((ch == '#') || ((ch == '"') || ((ch == '*') || ((ch == '^') || ((ch == '<') || ((ch == '>') || ((ch == '!') || ((ch == '+') || ((ch == '=') || ((ch == '$') || ((ch == '{') || ((ch == '}') || ((ch == '[') || ((ch == ']') || ((ch == '?') || ((ch == ':') || ((ch == '|') || ((ch == '`') || ((ch == '~') || ((ch == '\x00e9') || ((ch == '\x00e1') || ((ch == '\x00f3') || ((ch == '\x00fa') || ((ch == '\x00ed') || ((ch == '■') || ((ch == '▲') || ((ch == '\x00eb') || ((ch == '\x00e4') || ((ch == '\x00f6') || ((ch == '\x00fc') || ((ch == '\x00ef') || ((ch == '\x00c9') || ((ch == '\x00c1') || ((ch == '\x00d3') || ((ch == '\x00da') || ((ch == '\x00cd') || ((ch == '\x00ae') || ((ch == '\x00cb') || ((ch == '\x00c4') || ((ch == '\x00d6') || ((ch == '\x00dc') || (ch == '\x00cf')))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))) || (ch == '\x00d8'))
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }
    }
}
