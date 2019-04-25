using System.Linq;
using System.Text;

namespace RegExp.Extensions
{

    static class StringExtension
    {
        public static string RemoveAll(this string @string, params char[] @value)
        {
            StringBuilder result = new StringBuilder(@string.Length);

            for (int i = 0; i < @string.Length; i++)
            {
                if (!@value.Contains(@string[i]))
                    result.Append(@string[i]);
            }

            return result.ToString();
        }
    }
}
