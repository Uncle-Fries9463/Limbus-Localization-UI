using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limbus_Localization_UI
{
    public static class TextBases
    {
        public static bool ContainsStartsWith(this IEnumerable<string> array, string check)
        {
            foreach (var i in array)
            {
                if (i.StartsWith(check)) return true;
            }

            return false;
        }
        public static string Exform(this string Original, object Format) => Original.Replace("[№]", Convert.ToString(Format));
    }
}
