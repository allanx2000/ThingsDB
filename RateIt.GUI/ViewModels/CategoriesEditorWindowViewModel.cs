using RateIt.GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RateIt.GUI.ViewModels
{
    class CategoriesEditorWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private readonly Window window;
        public CategoriesEditorWindowViewModel(Window window)
        {
            this.window = window;

            LoadWindow();
        }

        public List<Category> Categories { get; private set; }
        public bool Changed { get; private set; }

        private void LoadWindow()
        {
            Categories = StateManager.Instance.DataStore.GetAllCategoriesWithCount();
        }
    }
}
