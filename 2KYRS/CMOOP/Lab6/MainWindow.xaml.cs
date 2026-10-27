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

namespace Lab6
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenCalculator_Click(object sender, RoutedEventArgs e)
        {
            var calc = new Calculator();
            calc.Show();
        }

        private void OpenBestOil_Click(object sender, RoutedEventArgs e)
        {
            var bestOil = new BestOil();
            bestOil.Show();
        }

        private void OpenCity_Click(object sender, RoutedEventArgs e)
        {
            var city = new City();
            city.Show();
        }
    }
}
