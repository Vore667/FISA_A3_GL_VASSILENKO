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
using WpfApp;

namespace interface_projet
{
    /// <summary>
    /// Logique d'interaction pour SelectionClientServer.xaml
    /// </summary>
    public partial class SelectionClientServer : Window
    {
        public SelectionClientServer()
        {
            InitializeComponent();
        }

        private void ServerButton_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre principale en mode Serveur
            MainWindow mainWindow = new MainWindow(isServerMode: true);
            mainWindow.Show();
            this.Close();
        }

        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre principale en mode Client
            MainWindow mainWindow = new MainWindow(isServerMode: false);
            mainWindow.Show();
            this.Close();
        }
    }
}
