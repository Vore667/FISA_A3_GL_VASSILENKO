using LogClassLibrary;
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
        LogController logController;

        public SettingsController(ILang langController, LogController logController)
        {
            this.langController = langController;
            this.logController = logController;
        }

        public void ChangeLanguage(string lang) => langController.ChangeLanguage(lang);

        public void SetLogType(string logType) => logController.SetLogType(logType);

        public void SetLogDirectory(string newDirectory) => logController.SetLogDirectory(newDirectory);
    }
}
