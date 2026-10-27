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
    public partial class Task4 : Window
    {
        public Task4()
        {
            InitializeComponent();
        }

        private void Toggle(UIElement element)
        {
            element.Visibility = element.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            Toggle(Btn1);
            Toggle(Btn2);
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            Toggle(Btn2);
            Toggle(Btn3);
            Toggle(Btn4);
        }

        private void Btn3_Click(object sender, RoutedEventArgs e)
        {
            Toggle(Btn3);
            Toggle(Btn5);
        }

        private void Btn4_Click(object sender, RoutedEventArgs e)
        {
            Toggle(Btn1);
            Toggle(Btn4);
        }

        private void Btn5_Click(object sender, RoutedEventArgs e)
        {
            Toggle(Btn2);
            Toggle(Btn5);
        }
    }
}
