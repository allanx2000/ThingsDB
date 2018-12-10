using System;
using System.IO;
using RateIt.GUI.Data;
using RateIt.GUI.Properties;

namespace RateIt.GUI
{
    internal class StateManager
    {
        private static StateManager instance;
        private static Settings settings;

        /// <summary>
        /// Loads a DB set in Settings
        /// </summary>
        internal static void Load()
        {
            settings = Settings.Default;

            while (string.IsNullOrEmpty(settings.DBPath))
            {
                var dlg = new SettingsWindow();
                dlg.ShowDialog();
            }

            instance = new StateManager();
        }

        public static StateManager Instance { get { return instance; } }
        public static Settings Settings { get { return settings; } }


        public IDataStore DataStore { get; private set; }

        private StateManager()
        {
            DataStore = new SQLDataStore(settings.DBPath);
        }
    }
}