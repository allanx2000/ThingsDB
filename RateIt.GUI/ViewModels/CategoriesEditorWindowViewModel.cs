using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using RateIt.GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
            RaisePropertyChanged("Categories");
        }

        public Category SelectedCategory
        {
            get { return Get<Category>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
                RaisePropertyChanged("CanDelete");

                CategoryName = value == null ? null : value.Name;
            }
        }

        public string CategoryName
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public bool CanDelete
        {
            get { return SelectedCategory != null; }
        }

        public ICommand NewCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    SelectedCategory = null;
                });
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new CommandHelper(DeleteCategory);
            }
        }

        private void DeleteCategory()
        {
            try
            {
                if (SelectedCategory == null)
                    return;
                else if (SelectedCategory.ItemCount > 0)
                {
                    throw new Exception("Cannot delete a category with items.");
                }
                else if (MessageBoxFactory.ShowConfirmAsBool($"Delete {SelectedCategory.Name}?", "Confirm Delete"))
                {
                    StateManager.Instance.DataStore.DeleteCategory(SelectedCategory.ID);

                    LoadWindow();
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new CommandHelper(SaveCategory);
            }
        }

        private void SaveCategory()
        {
            try
            {
                if (SelectedCategory == null)
                {
                    StateManager.Instance.DataStore.AddCategory(CategoryName);
                    CategoryName = null;
                }
                else
                {
                    SelectedCategory.Name = CategoryName;
                    StateManager.Instance.DataStore.UpdateCategory(SelectedCategory);
                }

                LoadWindow();
                Changed = true;
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }
    }
}
