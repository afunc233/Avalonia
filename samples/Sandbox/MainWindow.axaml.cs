using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input.TextInput;
using Avalonia.Markup.Xaml;
using Avalonia.Win32.WinRT.Composition;

namespace Sandbox
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            dwwdw.Click += Dwwdw_Click;
        }

        private void Dwwdw_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen; // or set this in XAML
            IsVisible = false;
            IsVisible = true;
        }
    }
}
