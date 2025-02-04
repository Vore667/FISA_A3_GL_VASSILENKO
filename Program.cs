using System;
using System.Threading;
using Projet_Easy_Save_grp_4.Controllers;

class Program
{
    static void Main()
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
            Console.WriteLine("╚══════════════════════════════════════════╝");

            Console.ResetColor();
            Console.Write($"{LangController.GetText("Menu_Option6")}");

            ConsoleKeyInfo key = Console.ReadKey();
            Console.Clear();

            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine($"{LangController.GetText("SubMenu_Option1")}");
                    Console.WriteLine($"{LangController.GetText("Overall_SubMenu_Option1")}");
                    ConsoleKeyInfo subKey0 = Console.ReadKey();
                    if (subKey0.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    // Appeler la classe qui gère ça
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    Console.WriteLine($"{LangController.GetText("SubMenu_Option2")}");
                    Console.WriteLine($"{LangController.GetText("Overall_SubMenu_Option1")}");
                    ConsoleKeyInfo subKey1 = Console.ReadKey();
                    if (subKey1.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    Console.Write($"{LangController.GetText("SubMenu_Option3")}");
                    string taskName = Console.ReadLine();
                    Console.Write($"{LangController.GetText("SubMenu_Option4")}");
                    string taskStartRepo = Console.ReadLine();
                    Console.Write($"{LangController.GetText("SubMenu_Option5")}");
                    string taskArrivalRepo = Console.ReadLine();
                    Console.Write($"{LangController.GetText("SubMenu_Option6")}");
                    string taskType = Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{LangController.GetText("SubMenu_Option7")}");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                    break;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    Console.WriteLine($"{LangController.GetText("SubMenu_Option8")}");
                    Console.WriteLine($"{LangController.GetText("Overall_SubMenu_Option1")}");
                    ConsoleKeyInfo subKey2 = Console.ReadKey();
                    if (subKey2.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                    Console.WriteLine($"{LangController.GetText("SubMenu_Option9")}");
                    string taskToDelete = Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{LangController.GetText("SubMenu_Option10")}");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                    break;

                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    Console.WriteLine($"{LangController.GetText("Select_Language")}");
                    Console.WriteLine("1. Français");
                    Console.WriteLine("2. English");
                    Console.Write($"{LangController.GetText("Menu_Option6")}");
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

                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"{LangController.GetText("Application_Exit")}");
                    Console.ResetColor();
                    Thread.Sleep(1000);
                    return;

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

