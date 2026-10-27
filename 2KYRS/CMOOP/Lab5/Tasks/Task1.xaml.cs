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
    public partial class Task1 : Window
    {
        public Task1()
        {
            InitializeComponent();
       
    }
        private void GreetingButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                StatusLabel.Text = button.Content.ToString();
            }
        }
    }
}
