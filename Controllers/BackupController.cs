using System;
using System.Collections.Generic;
using System.IO;

namespace Projet_Easy_Save_grp_4.Controllers
{
    internal class BackupController
    {
        private List<BackupTask> tasks;
        private const int MaxTasks = 5;

        public BackupController()
        {
            tasks = new List<BackupTask>();
        }

        public void AddBackup(string name, string source, string destination, string type)
        {
            if (tasks.Count >= MaxTasks)
            {
                Console.WriteLine("Erreur : Vous ne pouvez pas ajouter plus de 5 tâches de sauvegarde.");
                return;
            }

            if (!Directory.Exists(source))
            {
                Console.WriteLine("Erreur : Le répertoire source n'existe pas.");
                return;
            }

            tasks.Add(new BackupTask(name, source, destination, type));
            Console.WriteLine("Tâche de sauvegarde ajoutée avec succès.");
        }

        public void ListBackup()
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("Aucune tâche de sauvegarde disponible.");
                return;
            }

            foreach (var task in tasks)
            {
                Console.WriteLine($"Nom: {task.Name}, Type: {task.Type}, Source: {task.Source}, Destination: {task.Destination}");
            }
        }

        public void ExecuteBackup(string name)
        {
            BackupTask task = tasks.Find(t => t.Name == name);
            if (task == null)
            {
                Console.WriteLine("Erreur : Tâche de sauvegarde introuvable.");
                return;
            }

            task.Execute();
        }
    }

    internal class BackupTask
    {
        public string Name { get; }
        public string Source { get; }
        public string Destination { get; }
        public string Type { get; }

        public BackupTask(string name, string source, string destination, string type)
        {
            Name = name;
            Source = source;
            Destination = destination;
            Type = type;
        }

        public void Execute()
        {
            Console.WriteLine($"Exécution de la sauvegarde: {Name}");
            // Ajouter la logique pour copie complète ou différentielle
        }
    }
}