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
            private set {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public List<Item> Results {
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
                if (selectedTags == null || selectedTags.Count == 0)
                    return "(All)";
                else
                    return string.Join(", ", from x in selectedTags
                                             orderby x.Name ascending
                                             select x.Name);
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


        public ICommand OKCommand
        {
            get { return new CommandHelper(SetSettings); }
        }

        public ICommand CancelCommand
        {
            get { return new CommandHelper(() => window.Close()); }
        }

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

                window.Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        
        public ICommand ManageCategoriesCommand
        {
            get { return new CommandHelper(() =>
            {
                CategoriesEditorWindow ce = new CategoriesEditorWindow();
                ce.Owner = window;
                ce.ShowDialog();

                if (ce.Changed)
                    LoadWindow();
            }); }
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
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }
    }
}
