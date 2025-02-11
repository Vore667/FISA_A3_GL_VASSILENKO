using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projet_Easy_Save_grp_4.Resources;
using static Projet_Easy_Save_grp_4.Controllers.LangController;

namespace Projet_Easy_Save_grp_4.Interfaces
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
