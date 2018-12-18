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
using System.Windows.Shapes;

namespace RateIt.GUI
{
    /// <summary>
    /// Interaction logic for CategoriesEditorWindow.xaml
    /// </summary>
    public partial class TagsSelectorWindow : Window
    {
        private readonly TagsSelectorWindowViewModel vm;

        public TagsSelectorWindow(Category category, List<Tag> selected)
        {
            InitializeComponent();

            vm = new TagsSelectorWindowViewModel(this, category, selected);
            DataContext = vm;
        }

        public bool Changed { get { return vm.Changed; } }

        public List<Tag> GetSelectedTags()
        {
            return vm.GetSelectedTags();
        }

        private void UnselectedListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            vm.SelectCommand.Execute(null);
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            vm.UnselectCommand.Execute(null);
        }
    }
}
