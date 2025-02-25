using Console.Controllers;
using LogClassLibraryVue;
using System;


namespace Console.Views
{
    public static class View
    {

        internal static void DisplayMenu(BackupController backup, LogController logController) // Internal permet de rendre la méthode accessible uniquement dans le projet
        {
            bool isRunning = true; // Flag to control loop execution

            while (isRunning)
            {
                System.Console.Clear();
                System.Console.ForegroundColor = ConsoleColor.Cyan;
                int menuWidth = 40;

                System.Console.WriteLine("╔══════════════════════════════════════════╗");
                System.Console.WriteLine($"║ {LangController.GetText("Menu_Title").PadRight(menuWidth)} ║");
                System.Console.WriteLine("╠══════════════════════════════════════════╣");
                System.Console.WriteLine($"║ 1. {LangController.GetText("Menu_Option1").PadRight(menuWidth - 3)} ║");
                System.Console.WriteLine($"║ 2. {LangController.GetText("Menu_Option2").PadRight(menuWidth - 3)} ║");
                System.Console.WriteLine($"║ 3. {LangController.GetText("Menu_Option3").PadRight(menuWidth - 3)} ║");
                System.Console.WriteLine($"║ 4. {LangController.GetText("Menu_Option4").PadRight(menuWidth - 3)} ║");
                System.Console.WriteLine($"║ 5. {LangController.GetText("Menu_Option5").PadRight(menuWidth - 3)} ║");
                System.Console.WriteLine($"║ 6. {LangController.GetText("Menu_Option6").PadRight(menuWidth - 3)} ║");
                System.Console.WriteLine($"║ 7. {LangController.GetText("Menu_Option7").PadRight(menuWidth - 3)} ║");
                System.Console.WriteLine("╚══════════════════════════════════════════╝");
                System.Console.ResetColor();
                System.Console.Write($"{LangController.GetText("Menu_YourChoice")}");

                ConsoleKeyInfo key = System.Console.ReadKey();
                System.Console.Clear();
                HandleUserChoice(key, backup, logController, ref isRunning); // Permet de gérer les choix de l'utilisateur
            }
        }

        private static void HandleUserChoice(ConsoleKeyInfo key, BackupController backup, LogController logController, ref bool isRunning) // BackupsController et LogController sont passés en paramètre pour pouvoir les utiliser
        {
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    System.Console.WriteLine($"{LangController.GetText("SubMenu_ListOfExistingTasks")}");
                    backup.ListBackup();
                    WaitForUser();
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    AddBackup(backup);
                    break;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    ExecuteBackup(backup);
                    break;

                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    DeleteBackup(backup);
                    break;

                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    ChangeLanguage();
                    break;

                case ConsoleKey.D6:
                case ConsoleKey.NumPad6:
                    Parameters(logController); // Permet de changer le type de log (JSON ou XML)
                    break;

                case ConsoleKey.D7:
                case ConsoleKey.NumPad7:
                    System.Console.ForegroundColor = ConsoleColor.Magenta;
                    System.Console.WriteLine($"{LangController.GetText("Application_Exit")}");
                    System.Console.ResetColor();
                    Thread.Sleep(1000);
                    isRunning = false; // Stop loop instead of Environment.Exit
                    break;

                default:
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine($"{LangController.GetText("Invalid_Choice")}");
                    System.Console.ResetColor();
                    Thread.Sleep(1500);
                    break;
            }
        }

        private static void AddBackup(BackupController backup)
        {
            System.Console.WriteLine($"{LangController.GetText("Overall_SubMenu_Option1")}");
            ConsoleKeyInfo subkey = System.Console.ReadKey();
            if (subkey.Key == ConsoleKey.Escape)
            {
                return;
            }
            System.Console.WriteLine($"{LangController.GetText("SubMenu_CreatingTask")}");
            System.Console.Write($"{LangController.GetText("SubMenu_NameTask")}");
            string? taskName = System.Console.ReadLine();
            System.Console.Write($"{LangController.GetText("SubMenu_SourceDirectory")}");
            string? taskStartRepo = System.Console.ReadLine();
            System.Console.Write($"{LangController.GetText("SubMenu_DestDirectory")}");
            string? taskArrivalRepo = System.Console.ReadLine();
            System.Console.Write($"{LangController.GetText("SubMenu_TaskType")}");
            string? taskType = System.Console.ReadLine();
            backup.AddBackup(taskName, taskStartRepo, taskArrivalRepo, taskType);
            Thread.Sleep(2000);
        }

        private static void ExecuteBackup(BackupController backup)
        {
            System.Console.WriteLine($"{LangController.GetText("Overall_SubMenu_Option1")}");
            ConsoleKeyInfo subkey = System. Console.ReadKey();
            if (subkey.Key == ConsoleKey.Escape)
            {
                return;
            }
            System.Console.WriteLine($"{LangController.GetText("SubMenu_ExecutingTask")}");
            System.Console.WriteLine($"{LangController.GetText("SubMenu_ListOfExistingTasks")}");
            backup.ListBackup();
            System.Console.Write($"\n{LangController.GetText("SubMenu_EnterTaskNameToExecute")}");
            string? taskNameToExecute = System.Console.ReadLine();
            backup.ExecuteOrDeleteMultipleBackups(taskNameToExecute, true);
            WaitForUser();
        }

        private static void DeleteBackup(BackupController backup)
        {
            System.Console.WriteLine($"{LangController.GetText("Overall_SubMenu_Option1")}");
            ConsoleKeyInfo subkey = System.Console.ReadKey();
            if (subkey.Key == ConsoleKey.Escape)
            {
                return;
            }
            System.Console.WriteLine($"{LangController.GetText("SubMenu_DeletingTask")}");
            System.Console.WriteLine($"{LangController.GetText("SubMenu_ListOfExistingTasks")}");
            backup.ListBackup();
            System.Console.Write($"\n{LangController.GetText("SubMenu_EnterTaskNameToDelete")}");
            string? taskToDelete = System.Console.ReadLine();
            backup.ExecuteOrDeleteMultipleBackups(taskToDelete, false);
            Thread.Sleep(1500);
        }

        private static void ChangeLanguage()
        {
            System.Console.WriteLine($"{LangController.GetText("Select_Language")}");
            System.Console.WriteLine("1. Français");
            System.Console.WriteLine("2. English");
            System.Console.Write($"{LangController.GetText("Menu_YourChoice")}");
            string? langChoice = System.Console.ReadLine();
            if (langChoice == "1")
                LangController.SetLanguage("fr");
            else if (langChoice == "2")
                LangController.SetLanguage("en");
            else
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Write($"{LangController.GetText("Invalid_Choice")}");
                System.Console.ResetColor();
                Thread.Sleep(1500);
                return;
            }

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine($"{LangController.GetText("Update_Language")}");
            System.Console.ResetColor();
            Thread.Sleep(1500);
        }

        private static void Parameters(LogController logController) // LogController est passé en paramètre pour pouvoir le modifier
        {
            System.Console.WriteLine($"{LangController.GetText("Select_TypeOfLog")}");
            System.Console.WriteLine("1. JSON");
            System.Console.WriteLine("2. XML");
            System.Console.Write($"{LangController.GetText("Menu_YourChoice")}");
            string? logTypeChoice = System.Console.ReadLine();
            if (logTypeChoice == "1")
                logController.SetLogType("JSON");
            else if (logTypeChoice == "2")
                logController.SetLogType("XML");
            else
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Write($"{LangController.GetText("Invalid_Choice")}");
                System.Console.ResetColor();
                Thread.Sleep(1500);
                return;
            }
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine($"{LangController.GetText("Update_LogType")}");
            System.Console.ResetColor();
            Thread.Sleep(1500);
        }

        private static void WaitForUser()
        {
            System.Console.WriteLine($"\n{LangController.GetText("Overall_SubMenu_Option2")}");
            ConsoleKeyInfo subKey = System.Console.ReadKey();
            if (subKey.Key == ConsoleKey.Escape)
                return;
        }
    }
}
