using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Projet_Easy_Save_grp_4;
using Projet_Easy_Save_grp_4.Interfaces;
using Projet_Easy_Save_grp_4.Resources;



namespace Projet_Easy_Save_grp_4.Controllers
{
    class LangController : ILang
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

        public void ChangeLanguage(string langCode)
        {
            ResourceDictionary newResource = new ResourceDictionary();
            switch (langCode)
            {
                case "en":
                    newResource.Source = new Uri("Resources/Lang_en.xaml", UriKind.Relative);
                    break;
                case "fr":
                default:
                    newResource.Source = new Uri("Resources/Lang_fr.xaml", UriKind.Relative);
                    break;
            }

            //Ici on supprime les anciennes ressources et on charge les nouvelles
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(newResource);
        }
    }
}
