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
    public partial class Task2 : Window
    {
        public Task2()
        {
            InitializeComponent();
        }
        private void HideText_Click(object sender, RoutedEventArgs e)
        {
            InfoText.Visibility = Visibility.Collapsed;
        }

        private void ShowText_Click(object sender, RoutedEventArgs e)
        {
            InfoText.Visibility = Visibility.Visible;
        }
    }
}
