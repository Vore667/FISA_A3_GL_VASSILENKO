using System;
using LogClassLibrary;
using Projet_Easy_Save_grp_4.Controllers;
using Projet_Easy_Save_grp_4.Views;

namespace Projet_Easy_Save_grp_4
{
    static class Program
    {
        static void Main()
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            LogController logController = new LogController(logDirectory); // Instance unique de LogController pour tout le programme qui est utilisé dans View et BackupController
            BackupController backup = new BackupController(logDirectory, logController);

            View.DisplayMenu(backup, logController);
        }

    }
}
