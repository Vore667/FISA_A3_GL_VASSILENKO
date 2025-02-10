using System.Globalization;
using System.Resources;

namespace Projet_Easy_Save_grp_4.Resources
{
    public static class LangResources
    {
        private static ResourceManager resourceManager = new ResourceManager("Projet_Easy_Save_grp_4.Resources.Messages", typeof(LangResources).Assembly);

        public static string GetText(string key, string langCode)
        {
            CultureInfo culture = new CultureInfo(langCode);
            return resourceManager.GetString(key, culture) ?? key;
        }
    }
}
