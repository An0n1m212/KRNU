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

namespace Lab5.Tasks
{
    public partial class Task3 : Window
    {
        public Task3()
        {
            InitializeComponent();
        }
        private void HideInput_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Visibility = Visibility.Collapsed;
        }

        private void ShowInput_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Visibility = Visibility.Visible;
        }

        private void ClearInput_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Clear();
        }
    }
}
