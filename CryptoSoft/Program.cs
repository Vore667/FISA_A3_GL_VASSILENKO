using System;
using System.IO;
using CryptoSoftcConsole;

namespace CryptoSoft
{
    public class CryptoService
    {
        public static void Main()
        {
            Console.Title = "CryptoSoft - Chiffrement de fichiers";

            Console.WriteLine("===== CryptoSoft =====");
            Console.WriteLine("Ce programme chiffre un fichier avec une clé de votre choix.");
            Console.WriteLine();

            string cheminFichier;
            do
            {
                Console.Write("Entrez le chemin du fichier à chiffrer : ");
                cheminFichier = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(cheminFichier) || !File.Exists(cheminFichier))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Erreur : Le fichier spécifié n'existe pas. Veuillez réessayer.");
                    Console.ResetColor();
                }
            } while (string.IsNullOrWhiteSpace(cheminFichier) || !File.Exists(cheminFichier));

            string cle;
            do
            {
                Console.Write("Entrez la clé de chiffrement : ");
                cle = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(cle))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Erreur : La clé de chiffrement ne peut pas être vide. Veuillez réessayer.");
                    Console.ResetColor();
                }
            } while (string.IsNullOrWhiteSpace(cle));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nDémarrage du chiffrement...");
            Console.ResetColor();

            try
            {
                Transformer(cheminFichier, cle);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Chiffrement terminé avec succès !");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Une erreur est survenue : {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\nAppuyez sur une touche pour quitter...");
            Console.ReadKey();
        }

        public static void Transformer(string cheminFichier, string cle)
        {
            try
            {
                var gestionnaireFichier = new GestionnaireFichier(cheminFichier, cle);
                int tempsEcoule = gestionnaireFichier.TransformerFichier();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Temps écoulé pour le chiffrement : {tempsEcoule} ms");
                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erreur lors du chiffrement : {e.Message}");
                Console.ResetColor();
            }
        }
    }
}
