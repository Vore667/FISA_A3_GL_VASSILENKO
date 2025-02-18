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
using interface_projet.Properties;
using LogClassLibrary;




namespace Projet_Easy_Save_grp_4.Controllers
{
    public class LangController : ILang
    {
        private static LangController instance;
        public static LangController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LangController();
                }
                return instance;
            }
        }

        private LangController() {
            this.ChangeLanguage(Settings.Default.Language);
        }

        private static CultureInfo _currentCulture = new CultureInfo("fr"); // Langue par défaut

        // Pour les textes dans le CLI
        public static new void SetLanguage(string langCode)
        {
            _currentCulture = new CultureInfo(langCode);
        }

        public static new string GetText(string key)
        {
            return LangResources.GetText(key, _currentCulture.Name);
        } 
        
        public static string GetCurrentLanguage()
        {
            return _currentCulture.TwoLetterISOLanguageName; // Renvoie "fr" ou "en"
        }

        // Pour les textes dans la vue
        public void ChangeLanguage(string langCode)
        {
            // Charger le dictionnaire de ressources
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

            // Remplace le dictionnaire de ressources actuel
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(newResource);

            // Met à JOUR LA LANGUE
            _currentCulture = new CultureInfo(langCode);

            // Sauvegarde la langue choisie dans les paramètres de l'application dossier Properties fichier Settings.settings
            Settings.Default.Language = langCode;
            Settings.Default.Save();
        }

    }
}
