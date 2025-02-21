using System;
using LogClassLibraryVue;
using Console.Controllers;
using Console.Views;

namespace Console
{
    static class Program
    {
        static void Main()
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            LogController logController = LogController.Instance; // Correction de l'initialisation de logController
            logController.Initialize(logDirectory, "JSON"); // Instance unique de LogController pour tout le programme qui est utilisé dans View et BackupController
            BackupController backup = new BackupController(logDirectory, logController);

            View.DisplayMenu(backup, logController);
        }

    }
}
