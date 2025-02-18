using Projet_Easy_Save_grp_4.Controllers;
using Projet_Easy_Save_grp_4.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interface_projet.Controllers
{
    internal class SettingsController
    {
        ILang langController;

        public SettingsController(ILang langController)
        {
            this.langController = langController;
        }

        public void SetLanguage(string lang) => langController.SetLanguage(lang);
    }
}
