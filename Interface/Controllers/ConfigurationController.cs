using Projet_Easy_Save_grp_4.Models;
using System.IO;

namespace Projet_Easy_Save_grp_4.Controllers
{
    public class ConfigurationController
    {
        private readonly ConfigurationModel _configurationModel;
        private readonly string jsonFilePath;

        public ConfigurationController()
        {
            jsonFilePath = Path.Combine(GetProjectRoot(), "Resources", "settings.json");
            _configurationModel = new ConfigurationModel(jsonFilePath);
        }

        private static string GetProjectRoot()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo? directory = new DirectoryInfo(currentDirectory);

            while (directory != null && !directory.GetFiles("*.csproj").Any())
            {
                directory = directory.Parent;
            }

            return directory?.FullName ?? currentDirectory;
        }

        public List<string> GetEncryptExtensions() => _configurationModel.GetEncryptExtensions();

        public List<string> GetPriorityExtensions() => _configurationModel.GetPriorityExtensions();

        public string GetJobApp() => _configurationModel.GetJobApp();

        public void ModifyJobApp(string newJobApp) => _configurationModel.ModifyJobApp(newJobApp);
    }
}
