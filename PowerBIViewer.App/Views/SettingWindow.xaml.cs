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

// FILE: Views/SettingsWindow.xaml.cs
using PowerBIViewer.App.ViewModels;
using System.Windows;

namespace PowerBIViewer.App.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;

            // Luister naar het event van de ViewModel om dit venster te sluiten.
            viewModel.OnSaveComplete += () => this.Close();
        }
    }
}