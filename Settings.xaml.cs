using interface_projet.Controllers;
using Projet_Easy_Save_grp_4.Controllers;
using Projet_Easy_Save_grp_4.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace interface_projet
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private SettingsController settingsController;

        public Settings()
        {
            
            ILang langController = new LangController();
            settingsController = new SettingsController(langController);
            InitializeComponent();
            // Définir la langue par défaut en fonction de la langue actuelle
            SetDefaultLanguage();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void LanguageRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true)
            {
                string langCode = rb.Tag.ToString();
                // _settingsController est maintenant initialisé
                settingsController.SetLanguage(langCode);
                MessageBox.Show($"Langue changée en : {langCode}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SetDefaultLanguage()
        {
            // Obtenir la langue actuelle
            string currentLang = LangController.GetCurrentLanguage(); // Ajoutez une méthode pour récupérer la langue actuelle

            // Sélectionner le bon radio button
            if (currentLang == "fr")
                rbFrench.IsChecked = true;
            else if (currentLang == "en")
                rbEnglish.IsChecked = true;
        }

        private void UpdateUILanguage()
        {
            // Exemple de mise à jour des textes de l'UI après changement de langue
            this.Title = LangController.GetText("SettingsTitle");
            rbFrench.Content = LangController.GetText("French");
            rbEnglish.Content = LangController.GetText("English");
            // Ajoutez ici tous les autres éléments dont le texte doit être mis à jour
        }
    }
}
