using RateIt.GUI.Data;
using RateIt.GUI.Models;
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
        public MainWindow()
        {
            InitializeComponent();

            TestDB();
        }

        private void TestDB()
        {
            IDataStore ds = new SQLDataStore(@"E:\rateIt.db");
            /*
            Category cat = ds.AddCategory("Movies");

            Tag t1 = ds.AddTag(cat.ID, "Prime");
            Tag t2 = ds.AddTag(cat.ID, "Watched");

            Item item = new Item(cat, "Test Show");

            List<Tag> tags = new List<Tag>() { t1, t2 };
            item.Tags = tags;

            Item i = ds.AddItem(item);
            */
            Item item2 = ds.GetItem(1);

            string s = "";
        }
    }
}
