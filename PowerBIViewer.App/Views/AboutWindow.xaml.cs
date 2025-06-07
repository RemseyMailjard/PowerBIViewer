using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace PowerBIViewer.App.Views
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            LoadVersionInfo();
        }

        private void LoadVersionInfo()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version != null)
            {
                VersionTextBlock.Text = $"Versie {version.Major}.{version.Minor}.{version.Build}";
            }
            CopyrightTextBlock.Text = $"© {System.DateTime.Now.Year} Remsey Mailjard. All rights reserved.";
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}