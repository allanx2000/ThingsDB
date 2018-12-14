using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using RateIt.GUI.Data;
using RateIt.GUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace RateIt.GUI.ViewModels
{
    class MainWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private readonly Window window;

        public MainWindowViewModel(Window window)
        {
            this.window = window;

            LoadWindow();
        }

        private List<Category> categories;

        private void LoadWindow()
        {
            var ds = StateManager.Instance.DataStore;
            this.categories = ds.GetAllCategories();

            Categories = new CollectionViewSource();
            Categories.Source = categories;
            Categories.SortDescriptions.Add(new SortDescription("Value", ListSortDirection.Ascending));
        }

        public CollectionViewSource Categories
        {
            get { return Get<CollectionViewSource>(); }
            private set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public Item SelectedResultItem
        {
            get { return Get<Item>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public List<Item> Results
        {
            get { return Get<List<Item>>(); }
            private set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }


        public SearchCriteria CurrentQuery
        {
            get { return Get<SearchCriteria>(); }
            private set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }


        private List<Tag> selectedTags;

        public string SelectedTagsText
        {
            get
            {
                return Utils.TagsListToString(selectedTags);
            }
        }

        public Category SelectedCategory
        {
            get { return Get<Category>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }


        public string DBPath
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public string SearchName
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }


        public bool RatedOnly
        {
            get { return Get<bool>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public ICommand ManageCategoriesCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    CategoriesEditorWindow ce = new CategoriesEditorWindow();
                    ce.Owner = window;
                    ce.ShowDialog();

                    if (ce.Changed)
                        LoadWindow();
                });
            }
        }

        
        public ICommand DeleteItemCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    if (SelectedResultItem == null)
                        return;
                    else if (MessageBoxFactory.ShowConfirmAsBool($"Delete {SelectedResultItem.Name}?", "Delete Item", MessageBoxImage.Exclamation))
                    {
                        StateManager.Instance.DataStore.DeleteItem(SelectedResultItem);
                        Search();
                    }

                });
            }
        }

        public ICommand ManageTagsCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    TagsEditorWindow te = new TagsEditorWindow();
                    te.Owner = window;
                    te.ShowDialog();

                    if (te.Changed)
                        LoadWindow();
                });
            }
        }

        public ICommand EditSettingsCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    SettingsWindow sw = new SettingsWindow();
                    sw.Owner = window;
                    sw.ShowDialog();

                    if (sw.Changed)
                    {
                        StateManager.Load();
                        LoadWindow();
                    }
                });
            }
        }

        public ICommand SelectTagsCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
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

        public ICommand SearchCommand
        {
            get { return new CommandHelper(Search); }
        }

        private void Search()
        {
            try
            {
                SearchCriteria sc = new SearchCriteria(SelectedCategory, selectedTags, SearchName, RatedOnly);
                Results = StateManager.Instance.DataStore.Search(sc);

                CurrentQuery = sc;
                SelectedResultItem = null;
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand NewItemCommand
        {
            get { return new CommandHelper(() => EditItem()); }
        }

        public ICommand EditItemCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    if (SelectedResultItem != null)
                        EditItem(SelectedResultItem);
                });
            }
        }



        private void EditItem(Item existing = null)
        {
            ItemEditorWindow editor = new ItemEditorWindow(existing);
            editor.Owner = window;
            editor.ShowDialog();

            if (CurrentQuery != null)
                Search();
        }
    }
}
