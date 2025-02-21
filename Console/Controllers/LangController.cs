using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Console;
using Console.Interfaces;
using Console.Resources;



namespace Console.Controllers
{
    internal class LangController : ILang
    {
        private static CultureInfo _currentCulture = new CultureInfo("fr"); // Langue par défaut

        public static new void SetLanguage(string langCode)
        {
            _currentCulture = new CultureInfo(langCode);
        }

        public static new string GetText(string key)
        {
            return LangResources.GetText(key, _currentCulture.Name);
        }
    }
}
