﻿using System;
using Projet_Easy_Save_grp_4.Controllers;


namespace Projet_Easy_Save_grp_4.Views
{
    public static class View
    {
        internal static void DisplayMenu(BackupController backup)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                int menuWidth = 40;

                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine($"║ {LangController.GetText("Menu_Title").PadRight(menuWidth)} ║");
                Console.WriteLine("╠══════════════════════════════════════════╣");
                Console.WriteLine($"║ 1. {LangController.GetText("Menu_Option1").PadRight(menuWidth - 3)} ║");
                Console.WriteLine($"║ 2. {LangController.GetText("Menu_Option2").PadRight(menuWidth - 3)} ║");
                Console.WriteLine($"║ 3. {LangController.GetText("Menu_Option3").PadRight(menuWidth - 3)} ║");
                Console.WriteLine($"║ 4. {LangController.GetText("Menu_Option4").PadRight(menuWidth - 3)} ║");
                Console.WriteLine($"║ 5. {LangController.GetText("Menu_Option5").PadRight(menuWidth - 3)} ║");
                Console.WriteLine($"║ 6. {LangController.GetText("Menu_Option6").PadRight(menuWidth - 3)} ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();
                Console.Write($"{LangController.GetText("Menu_YourChoice")}");

                ConsoleKeyInfo key = Console.ReadKey();
                Console.Clear();
                HandleUserChoice(key, backup);
            }
        }

        private static void HandleUserChoice(ConsoleKeyInfo key, BackupController backup)
        {
            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine($"{LangController.GetText("SubMenu_ListOfExistingTasks")}");
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
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"{LangController.GetText("Application_Exit")}");
                    Console.ResetColor();
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{LangController.GetText("Invalid_Choice")}");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                    break;
            }
        }

        private static void AddBackup(BackupController backup)
        {
            Console.WriteLine($"{LangController.GetText("SubMenu_CreatingTask")}");
            Console.Write($"{LangController.GetText("SubMenu_NameTask")}");
            string? taskName = Console.ReadLine();
            Console.Write($"{LangController.GetText("SubMenu_SourceDirectory")}");
            string? taskStartRepo = Console.ReadLine();
            Console.Write($"{LangController.GetText("SubMenu_DestDirectory")}");
            string? taskArrivalRepo = Console.ReadLine();
            Console.Write($"{LangController.GetText("SubMenu_TaskType")}");
            string? taskType = Console.ReadLine();
            backup.AddBackup(taskName, taskStartRepo, taskArrivalRepo, taskType);
            Thread.Sleep(2000);
        }

        private static void ExecuteBackup(BackupController backup)
        {
            Console.WriteLine($"{LangController.GetText("SubMenu_ExecutingTask")}");
            Console.WriteLine($"{LangController.GetText("SubMenu_ListOfExistingTasks")}");
            backup.ListBackup();
            Console.Write($"\n{LangController.GetText("SubMenu_EnterTaskNameToExecute")}");
            string? taskNameToExecute = Console.ReadLine();
            backup.ExecuteOrDeleteMultipleBackups(taskNameToExecute, true);
            WaitForUser();
        }

        private static void DeleteBackup(BackupController backup)
        {
            Console.WriteLine($"{LangController.GetText("SubMenu_DeletingTask")}");
            Console.WriteLine($"{LangController.GetText("SubMenu_ListOfExistingTasks")}");
            backup.ListBackup();
            Console.Write($"\n{LangController.GetText("SubMenu_EnterTaskNameToDelete")}");
            string? taskToDelete = Console.ReadLine();
            backup.ExecuteOrDeleteMultipleBackups(taskToDelete, false);
            Thread.Sleep(1500);
        }

        private static void ChangeLanguage()
        {
            Console.WriteLine($"{LangController.GetText("Select_Language")}");
            Console.WriteLine("1. Français");
            Console.WriteLine("2. English");
            Console.Write($"{LangController.GetText("Menu_YourChoice")}");
            string? langChoice = Console.ReadLine();
            if (langChoice == "1")
                LangController.SetLanguage("fr");
            else if (langChoice == "2")
                LangController.SetLanguage("en");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{LangController.GetText("Update_Language")}");
            Console.ResetColor();
            Thread.Sleep(1500);
        }

        private static void WaitForUser()
        {
            Console.WriteLine($"\n{LangController.GetText("Overall_SubMenu_Option2")}");
            ConsoleKeyInfo subKey = Console.ReadKey();
            if (subKey.Key == ConsoleKey.Escape)
                return;
        }
    }
}
