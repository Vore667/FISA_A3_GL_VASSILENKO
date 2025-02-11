using System;
using Projet_Easy_Save_grp_4.Controllers;
using Projet_Easy_Save_grp_4.Views;

namespace Projet_Easy_Save_grp_4
{
    static class Program
    {
        static void Main()
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            BackupController backup = new BackupController(logDirectory);

            View.DisplayMenu(backup);
        }
    }
}
