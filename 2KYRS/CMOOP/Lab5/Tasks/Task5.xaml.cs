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
    public partial class Task5 : Window
    {
        public Task5()
        {
            InitializeComponent();
        }
        private readonly Random _random = new Random();

        private void CatchMeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            double maxX = GameCanvas.ActualWidth - CatchMeButton.ActualWidth;
            double maxY = GameCanvas.ActualHeight - CatchMeButton.ActualHeight;

            if (maxX > 0 && maxY > 0)
            {
                double newX = _random.NextDouble() * maxX;
                double newY = _random.NextDouble() * maxY;

                Canvas.SetLeft(CatchMeButton, newX);
                Canvas.SetTop(CatchMeButton, newY);
            }
        }

        private void CatchMeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вітаємо! Ви спіймали кнопку и перемогли!", "Перемога");
        }

    }
}
