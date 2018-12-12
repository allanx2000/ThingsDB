using System;
using System.IO;
using RateIt.GUI.Data;
using RateIt.GUI.Properties;

namespace RateIt.GUI
{

    internal class StateManager
    {

        /// <summary>
        /// Loads a DB set in Settings
        /// </summary>
        internal static void Load()
        {
            Settings = Settings.Default;

            while (string.IsNullOrEmpty(Settings.DBPath))
            {
                var dlg = new SettingsWindow();
                dlg.ShowDialog();
            }

            Instance = new StateManager();
        }

        public static StateManager Instance { get; private set; }
        public static Settings Settings { get; private set; }


        public IDataStore DataStore { get; private set; }

        private StateManager()
        {
            DataStore = new SQLDataStore(Settings.DBPath);
        }
    }
}