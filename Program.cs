using System;
using Projet_Easy_Save_grp_4.Controllers;
using Projet_Easy_Save_grp_4.Views;
using System.IO;
using LogClassLibrary;


namespace Projet_Easy_Save_grp_4
{
    static class Program
    {
        static void Programmain()
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            string logType = "JSON";
            LangController langController = LangController.Instance;
            LogController logController = LogController.Instance;// Instance unique de LogController pour tout le programme qui est utilisé dans View et BackupController
            BackupController backup = new BackupController(logDirectory, logController);

            View.DisplayMenu(backup, logController);
        }
    }
}
