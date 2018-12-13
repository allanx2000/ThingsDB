using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using RateIt.GUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace RateIt.GUI.ViewModels
{
    class TagsSelectorWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private readonly Window window;


        private ObservableCollection<Tag> unselectedTags = new ObservableCollection<Tag>();
        private ObservableCollection<Tag> selectedTags = new ObservableCollection<Tag>();

        public List<Tag> GetSelectedTags()
        {
            return selectedTags.ToList();
        }

        public TagsSelectorWindowViewModel(Window window, Category category, List<Tag> tags)
        {
            this.window = window;
            this.SelectedCategory = category;


            var sd = new SortDescription("Name", ListSortDirection.Ascending);

            SelectedTags = new CollectionViewSource();
            SelectedTags.Source = selectedTags;
            SelectedTags.SortDescriptions.Add(sd);

            UnselectedTags = new CollectionViewSource();
            UnselectedTags.Source = unselectedTags;
            UnselectedTags.SortDescriptions.Add(sd);

            if (tags != null)
            {
                foreach (var t in tags)
                {
                    selectedTags.Add(t);
                }
            }

            RefreshTags();
        }

        private void RefreshTags()
        {   

            var tags = StateManager.Instance.DataStore.GetAllTagsWithCount(SelectedCategory.ID);

            unselectedTags.Clear();

            foreach (var t in tags)
            {
                if (!selectedTags.Contains(t))
                    unselectedTags.Add(t);
            }

            SelectedTag = UnselectedTag = null;
        }

        public Category SelectedCategory { get; private set; }

        public CollectionViewSource SelectedTags
        {
            get { return Get<CollectionViewSource>(); }
            private set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public CollectionViewSource UnselectedTags
        {
            get { return Get<CollectionViewSource>(); }
            private set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public Tag SelectedTag
        {
            get { return Get<Tag>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public Tag UnselectedTag
        {
            get { return Get<Tag>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public ICommand SelectCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    if (UnselectedTag != null)
                    {
                        selectedTags.Add(UnselectedTag);
                        unselectedTags.Remove(UnselectedTag);
                        UnselectedTag = null;
                    }
                });
            }
        }

        public ICommand UnselectCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    if (SelectedTag != null)
                    {
                        unselectedTags.Add(SelectedTag);
                        selectedTags.Remove(SelectedTag);
                        SelectedTag = null;
                    }
                });
            }
        }

        public ICommand OKCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    Changed = true;
                    window.Close();
                });
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new CommandHelper(() =>
                {
                    window.Close();
                });
            }
        }

        public bool Changed { get; private set; }
    }
}
