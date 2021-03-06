﻿using RateIt.GUI.Models;
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
    /// Interaction logic for ItemEditorWindow.xaml
    /// </summary>
    public partial class ItemEditorWindow : Window
    {
        private ItemEditorWindowViewModel vm;

        public ItemEditorWindow(Item i = null)
        {
            InitializeComponent();

            this.vm = new ItemEditorWindowViewModel(this, i);
            DataContext = vm;
        }
    }
}
