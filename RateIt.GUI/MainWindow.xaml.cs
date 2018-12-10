using RateIt.GUI.Data;
using RateIt.GUI.Models;
using RateIt.GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RateIt.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel vm;

        public MainWindow()
        {
            InitializeComponent();
            StateManager.Load();

            vm = new MainWindowViewModel(this);
            DataContext = vm;

            //TestDB();
        }



        private void TestDB()
        {
            IDataStore ds = StateManager.Instance.DataStore;
            if (ds.GetCategory(1) == null)
            {
                Category cat = ds.AddCategory("Movies");

                Tag t1 = ds.AddTag(cat.ID, "Prime");
                Tag t2 = ds.AddTag(cat.ID, "Watched");

                Item item = new Item(cat, "Test Show");

                item.Tags = new List<Tag>() { t1, t2 };
                ds.UpsertItem(item);

                Item item2 = new Item(cat, "Test2");

                item2.Tags = new List<Tag>();
                item2.Tags.Add(ds.GetTag(1));
                ds.UpsertItem(item2);
            }

            var items = ds.GetItemsForTag(1);

            string s = "";
        }
    }
}
