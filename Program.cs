// See https://aka.ms/new-console-template for more information
using System;
using Projet_Easy_Save_grp_4;


class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("1. Lister les tâches de sauvegarde");
            Console.WriteLine("2. Créer une tâche de sauvegarde");
            Console.WriteLine("3. Supprimer une/des tâche(s) de sauvegarde");
            Console.WriteLine("4. Changer de langue");
            Console.WriteLine("5. Quitter");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Clear();
                Console.WriteLine("Ensemble des tâches de sauvegarde existantes");
                // Print les tâches
                Console.WriteLine("Recopiez le nom de la/les tâche(s) que vous souhaitez executer");
                string task = Console.ReadLine();
                // Appeler la classe qui gère ça

                //Si pr X ou Y raison ça marche pas
                Console.WriteLine("Erreur de création de la/les tâche(s) de sauvegarde, retour au menu");
                //sleep
                Console.Clear();
            }
            else if (choice == "2")
            {
                Console.Clear();
                // Appeler classe pr voir si y a déjà 5 tâche qui ont été crée
                // Si c'est le cas dire erreur y a deja 5 tâches
                // Sinon continuer dans la création de la tâche.
                Console.WriteLine("Création d'une tâche de sauvegarde");
                Console.WriteLine("Nom de la tâche");
                string task_name = Console.ReadLine();
                Console.WriteLine("Repertoire de départ");
                string task_start_repo = Console.ReadLine();
                Console.WriteLine("Repertoire d'arrivé");
                string task_arrival_repo = Console.ReadLine();
                Console.WriteLine("Type de tâche : 1 pour complète, 2 pour incrémentielle");
                string task_type = Console.ReadLine();
                // Verif si la tâche a été crée (vérif que les chemins sont bons si oui continuer
                Console.WriteLine("La tâche a été crée avec succès");
                // si non dire erreur et revenir au menu
                Console.WriteLine("Erreur de la/les tâche(s) de sauvegarde, retour au menu");
                //sleep
                Console.Clear();
            }
            else if (choice == "3")
            {
                Console.Clear();
                Console.WriteLine("Ensemble des tâches de sauvegarde existantes");
                // Print les tâches
                Console.WriteLine("Recopiez le nom de la/les tâche(s) que vous souhaitez supprimer");
                string task = Console.ReadLine();

            }
            else if (choice == "4")
            {
                Console.Clear();
                Console.WriteLine("Quelle langue souhaitez vous sélectionner");
                // Afficher les différentes langues dispo
                // Puis revenir au menu avec la nv langue

            }
            else if (choice == "5")
            {
                Console.Clear();
                Console.WriteLine("Fermeture de l'application");
                break;
            }
            else 
            {
                Console.Write("Erreur, choix invalide, retour au menu.");
            }
        }
    }
}