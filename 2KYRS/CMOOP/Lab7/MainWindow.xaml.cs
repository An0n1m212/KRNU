using System.Windows;
using Lab7.Task1.Views;
using Lab7.Task2.Views;
using Lab7.Task3.Views;


namespace Lab7
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenAuth_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Login();
            loginWindow.ShowDialog();
        }

        private void Open2048_Click(object sender, RoutedEventArgs e)
        {
            var gameWindow = new Game2048();
            gameWindow.ShowDialog();
        }

        private void OpenSniper_Click(object sender, RoutedEventArgs e)
        {
            var sniperWindow = new NightSniper();
            sniperWindow.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
