using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console.Resources;
using static Console.Controllers.LangController;

namespace Console.Interfaces
{
    internal interface ILang
    {
        public static void SetLanguage(string langCode)
        {

        }

        public static string? GetText(string key)
        {

            return null;
        }
    }
}
