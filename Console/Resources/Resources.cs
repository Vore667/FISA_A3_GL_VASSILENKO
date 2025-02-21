using System.Globalization;
using System.Resources;

namespace Console.Resources
{
    public static class LangResources
    {
        private static ResourceManager resourceManager = new ResourceManager("Console.Resources.Messages", typeof(LangResources).Assembly);

        public static string GetText(string key, string langCode)
        {
            CultureInfo culture = new CultureInfo(langCode);
            return resourceManager.GetString(key, culture) ?? key;
        }
    }
}
