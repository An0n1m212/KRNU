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

using Lab5.Tasks; 

namespace Lab5
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenTask1_Click(object sender, RoutedEventArgs e) => new Task1().Show();
        private void OpenTask2_Click(object sender, RoutedEventArgs e) => new Task2().Show();
        private void OpenTask3_Click(object sender, RoutedEventArgs e) => new Task3().Show();
        private void OpenTask4_Click(object sender, RoutedEventArgs e) => new Task4().Show();
        private void OpenTask5_Click(object sender, RoutedEventArgs e) => new Task5().Show();
        private void OpenTask6_Click(object sender, RoutedEventArgs e) => new Task6().Show();
        private void OpenTask7_Click(object sender, RoutedEventArgs e) => new Task7().Show();
    }
}
