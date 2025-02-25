using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Console.Interfaces;

namespace Console.Other
{
    public class ConfigurationService : IConfigurationService
    {
        public void LoadConfig()
        {
            // Implémentation du chargement de la configuration
            System.Console.WriteLine("Configuration chargée.");
        }

        public string GetSetting()
        {
            // Retourne une configuration fictive en JSON
            return "{\"setting\": \"value\"}";
        }
    }
}
