using Microsoft.Win32;
using System.Windows;

namespace WpfApp
{
    public partial class AjouterFenetre : Window
    {
        public AjouterFenetre()
        {
            InitializeComponent();
        }

        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSource_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                lblSource.Content = dlg.FileName;
            }
        }

        private void BtnDestination_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                lblDestination.Content = dlg.FileName;
            }
        }
    }
}
