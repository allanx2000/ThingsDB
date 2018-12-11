using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RateIt.GUI.ViewModels
{
    class SettingsWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {


        public SettingsWindowViewModel(Window window)
        {
            this.window = window;
            DBPath = StateManager.Settings.DBPath; 
        }

        private readonly Window window;

        public string DBPath
        {
            get { return Get<string>(); }
            set {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public ICommand OKCommand
        {
            get { return new CommandHelper(SetSettings); }
        }

        public ICommand CancelCommand
        {
            get { return new CommandHelper(() => window.Close()); }
        }

        public bool Changed { get; private set; }

        private void SetSettings()
        {
            try
            {
                if (string.IsNullOrEmpty(DBPath))
                {
                    throw new Exception("DBPath is required.");
                }
                else if (!Path.IsPathRooted(DBPath))
                {
                    throw new Exception("Path must be absolute.");
                }

                StateManager.Settings.DBPath = DBPath;
                StateManager.Settings.Save();

                Changed = true;
                window.Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }
    }
}
