using System.Configuration;
using System.Data;
using System.Windows;

namespace interface_projet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(newResource);
        }
    }

}
