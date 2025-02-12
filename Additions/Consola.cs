using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limbus_Localization_UI.Additions
{
    internal class Consola
    {
        public static Dictionary<string, string> CS = new()
        {
            ["x0"] = "\x1b[38;5;243m",
            ["x1"] = "\x1b[38;5;214m",
            ["def"] = "\x1b[0m",
        };

        public static void rin(params object[] s) => Console.WriteLine(String.Join(' ', s));
    }
}
