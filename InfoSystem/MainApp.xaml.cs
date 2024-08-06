using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace InfoSystem
{
    public partial class MainApp : Window
    {
        public MainApp()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            BorderThickness = new Thickness(WindowState == WindowState.Maximized ? 8 : 0);
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
