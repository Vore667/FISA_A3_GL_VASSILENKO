using System;
using System.Windows;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Ajuste la taille à 2/3 de l'écran
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            this.Width = screenWidth * (2.0 / 3.0);
            this.Height = screenHeight * (2.0 / 3.0);
            this.Left = (screenWidth - this.Width) / 2;
            this.Top = (screenHeight - this.Height) / 2;
        }

        private void BtnParametres_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ouvrir les paramètres !");
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            AjouterFenetre fenetre = new AjouterFenetre();
            fenetre.ShowDialog();
        }
    }
}
