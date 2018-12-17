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
    class TagsEditorWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private readonly Window window;
        public TagsEditorWindowViewModel(Window window)
        {
            this.window = window;

            LoadWindow();
        }

        public List<Category> Categories { get; private set; }

        public List<Tag> Tags
        {
            get { return Get<List<Tag>>(); }
            private set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public bool Changed { get; private set; }

        private void LoadWindow(int? categoryId = null)
        {
            Categories = StateManager.Instance.DataStore.GetAllCategoriesWithCount();
            RaisePropertyChanged("Categories");

            if (categoryId == null)
                SelectedCategory = Categories.First();
            else
            {
                SelectedCategory = Categories.First(x => x.ID == categoryId);
            }
        }

        private void LoadTags(int categoryId)
        {
            Tags = StateManager.Instance.DataStore.GetAllTagsWithCount(categoryId);
        }

        public Category SelectedCategory
        {
            get { return Get<Category>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();

                if (value != null)
                {
                    LoadTags(value.ID);
                }
            }
        }

        public Tag SelectedTag
        {
            get { return Get<Tag>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
                RaisePropertyChanged("CanDelete");

                TagName = value == null ? null : value.Name;
            }
        }

        public string TagName
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
            get { return SelectedTag != null; }
        }

        public ICommand NewCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    SelectedTag = null;
                });
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new CommandHelper(DeleteTag);
            }
        }

        private void DeleteTag()
        {
            try
            {
                if (SelectedTag == null)
                    return;
                else if (MessageBoxFactory.ShowConfirmAsBool($"Delete {SelectedTag.Name}?", "Confirm Delete"))
                {
                    StateManager.Instance.DataStore.DeleteTag(SelectedTag.ID);

                    LoadWindow(SelectedCategory.ID);
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand CloseCommand
        {
            get { return new CommandHelper(() => window.Close()); }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new CommandHelper(SaveTag);
            }
        }

        private void SaveTag()
        {
            try
            {
                if (SelectedCategory == null)
                    return;

                if (SelectedTag == null)
                {
                    StateManager.Instance.DataStore.AddTag(SelectedCategory.ID, TagName);
                    TagName = null;
                }
                else
                {
                    SelectedTag.Name = TagName;
                    StateManager.Instance.DataStore.UpdateTag(SelectedTag);
                }

                LoadWindow(SelectedCategory.ID);

                Changed = true;
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }
    }
}
