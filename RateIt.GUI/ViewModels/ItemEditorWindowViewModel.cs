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
    class ItemEditorWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private readonly Window window;
        private readonly Item existing;

        public ItemEditorWindowViewModel(ItemEditorWindow window, Item existing = null)
        {
            this.window = window;
            this.existing = existing;

            LoadWindow();
        }

        public List<Category> Categories {
            get { return Get<List<Category>>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }


        private List<Tag> selectedTags;

        public string SelectedTagsText
        {
            get { return Utils.TagsListToString(selectedTags, ""); }
        }

        private void LoadWindow()
        {
            Categories = StateManager.Instance.DataStore.GetAllCategoriesWithCount();
            
            if (existing != null)
            {
                SelectedCategory = existing.Category;
                SetSelectedTags(existing.Tags);
                Name = existing.Name;
                ID = existing.ID;

                Notes = existing.Notes;
                URL = existing.URL;
                Rating = existing.Rating;
            }
        }

        public string Title
        {
            get { return (existing == null ? "Add" : "Edit") + " Item"; }
        }

        private void SetSelectedTags(List<Tag> tags)
        {
            selectedTags = tags;
            RaisePropertyChanged("SelectedTagsText");
        }

        public Category SelectedCategory
        {
            get { return Get<Category>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();

                SetSelectedTags(null);
            }
        }

        public string Name
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public int ID { get; private set; }


        public string Notes
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public string RatingText
        {
            get { return Rating == 0 ? "NA" : Rating.ToString(); }
        }

        public int Rating
        {
            get { return Get<int>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
                RaisePropertyChanged("RatingText");
            }
        }
        public string URL
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }


        public ICommand CancelCommand
        {
            get
            {
                return new CommandHelper(() => window.Close());
            }
        }

        public ICommand SelectTagsCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    if (SelectedCategory == null)
                    {
                        MessageBoxFactory.ShowError("A category must be selected first.", "Category Not Set", owner: window);
                        return;
                    }

                    TagsSelectorWindow selector = new TagsSelectorWindow(SelectedCategory, selectedTags);
                    selector.Owner = window;
                    selector.ShowDialog();

                    if (selector.Changed)
                    {
                        selectedTags = selector.GetSelectedTags();
                        RaisePropertyChanged("SelectedTagsText");
                    }
                });
            }
        }

        public ICommand ClearTagsCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    selectedTags = null;
                    RaisePropertyChanged("SelectedTagsText");
                });
            }
        }


        public ICommand SaveCommand
        {
            get
            {
                return new CommandHelper(SaveItem);
            }
        }

        private void SaveItem()
        {
            try
            {
                if (existing == null)
                {
                    if (SelectedCategory == null || string.IsNullOrEmpty(Name))
                    {
                        throw new Exception("Category and Name are required.");
                    }

                    Item x = new Item(SelectedCategory, Name);
                    x.Tags = selectedTags;
                    x.Notes = Notes;
                    x.Rating = Rating;
                    x.URL = URL;


                    StateManager.Instance.DataStore.UpsertItem(x);
                }
                else
                {
                    existing.Name = Name;
                    existing.Tags = selectedTags;
                    existing.Category = SelectedCategory;
                    existing.Notes = Notes;
                    existing.Rating = Rating;
                    existing.URL = URL;
                    StateManager.Instance.DataStore.UpsertItem(existing);
                }

                window.Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }
    }
}
