using System;
using System.Threading;
using Projet_Easy_Save_grp_4.Controllers;

class Program
{
    static void Main()
    {

        BackupController Backup  = new BackupController();

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

            switch (key.Key)
            {
                //Lister les backups
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine($"{LangController.GetText("SubMenu_ListOfExistingTasks")}");
                    Backup.ListBackup();
                    Console.WriteLine($"\n{LangController.GetText("Overall_SubMenu_Option2")}");
                    ConsoleKeyInfo subKey001 = Console.ReadKey();
                    if (subKey001.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    break;


                //Ajouter une backup
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    Console.WriteLine($"{LangController.GetText("SubMenu_CreatingTask")}");
                    Console.WriteLine($"{LangController.GetText("Overall_SubMenu_Option1")}");
                    ConsoleKeyInfo subKey1 = Console.ReadKey();
                    if (subKey1.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    Console.Write($"{LangController.GetText("SubMenu_NameTask")}");
                    string taskName = Console.ReadLine();
                    Console.Write($"{LangController.GetText("SubMenu_SourceDirectory")}");
                    string taskStartRepo = Console.ReadLine();
                    Console.Write($"{LangController.GetText("SubMenu_DestDirectory")}");
                    string taskArrivalRepo = Console.ReadLine();
                    Console.Write($"{LangController.GetText("SubMenu_TaskType")}");
                    string taskType = Console.ReadLine();
                    Backup.AddBackup(taskName, taskStartRepo, taskArrivalRepo, taskType);
                    Thread.Sleep(2000);
                    break;


                //Executer une backup
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    Console.WriteLine($"{LangController.GetText("SubMenu_ExecutingTask")}");
                    Console.WriteLine($"{LangController.GetText("Overall_SubMenu_Option1")}");
                    ConsoleKeyInfo subKey01 = Console.ReadKey();
                    if (subKey01.Key == ConsoleKey.Escape)
                    {
                        break;
                    }

                    Console.WriteLine($"\n{LangController.GetText("SubMenu_ListOfExistingTasks")}");
                    Backup.ListBackup();

                    Console.Write($"\n{LangController.GetText("SubMenu_EnterTaskNameToExecute")}");
                    string taskNameToExecute = Console.ReadLine();

                    Backup.ExecuteOrDeleteMultipleBackups(taskNameToExecute, true);

                    Console.WriteLine($"\n{LangController.GetText("Overall_SubMenu_Option2")}");
                    ConsoleKeyInfo subKey02 = Console.ReadKey();
                    if (subKey02.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    break;



                //Supprimer une backup
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    Console.WriteLine($"{LangController.GetText("SubMenu_DeletingTask")}");
                    Console.WriteLine($"{LangController.GetText("Overall_SubMenu_Option1")}");
                    ConsoleKeyInfo subKey2 = Console.ReadKey();
                    if (subKey2.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    Console.WriteLine($"{LangController.GetText("SubMenu_ListOfExistingTasks")}");
                    Backup.ListBackup();
                    Console.Write($"\n{LangController.GetText("SubMenu_EnterTaskNameToDelete")}");
                    string taskToDelete = Console.ReadLine();
                    Backup.ExecuteOrDeleteMultipleBackups(taskToDelete, false);
                    Thread.Sleep(1500);
                    break;


                //Changer la langue
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    Console.WriteLine($"{LangController.GetText("Select_Language")}");
                    Console.WriteLine("1. Français");
                    Console.WriteLine("2. English");
                    Console.Write($"{LangController.GetText("Menu_YourChoice")}");
                    string langChoice = Console.ReadLine();
                    if (langChoice == "1")
                    {
                        LangController.SetLanguage("fr");
                    }
                    else if (langChoice == "2")
                    {
                        LangController.SetLanguage("en");
                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{LangController.GetText("Update_Language")}");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                    break;
                
                 //Quitter l'application
                case ConsoleKey.D6:
                case ConsoleKey.NumPad6:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"{LangController.GetText("Application_Exit")}");
                    Console.ResetColor();
                    Thread.Sleep(1000);
                    return;

                //Choix invalide
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{LangController.GetText("Invalid_Choice")}");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                    break;
            }
        }

    }
}

