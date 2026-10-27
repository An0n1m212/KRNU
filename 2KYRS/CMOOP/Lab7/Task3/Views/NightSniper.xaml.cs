using System;
using System.Windows;
using System.Windows.Controls;

namespace Lab7.Task3.Views
{
    public partial class NightSniper : Window
    {
        private readonly Button[,] gridButtons = new Button[4, 4];
        private int targetRow;
        private int targetCol;
        private readonly Random rand = new Random();
        private bool isGameOver = false;

        public NightSniper()
        {
            InitializeComponent();
            BuildGrid();
            StartGame();
        }

        private void BuildGrid()
        {
            SniperGrid.Children.Clear();
            var template = (ControlTemplate)Application.Current.Resources["FlashlightButtonTemplate"];

            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    var btn = new Button
                    {
                        Template = template,
                        Margin = new Thickness(4),
                        Tag = (r, c)
                    };
                    btn.Click += Tile_Click;
                    gridButtons[r, c] = btn;
                    SniperGrid.Children.Add(btn);
                }
            }
        }

        private void StartGame()
        {
            isGameOver = false;
            targetRow = rand.Next(0, 4);
            targetCol = rand.Next(0, 4);

            var template = (ControlTemplate)Application.Current.Resources["FlashlightButtonTemplate"];

            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    var btn = gridButtons[r, c];
                    btn.Style = null;
                    btn.Template = template;
                    btn.Content = null;
                    btn.IsEnabled = true;
                }
            }
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            if (isGameOver || sender is not Button btn) return;

            var (r, c) = ((int, int))btn.Tag;

            if (r == targetRow && c == targetCol)
            {
                btn.Style = (Style)Application.Current.Resources["HitButtonStyle"];
                MessageBox.Show("Влучання! Ви знайшли приховану ціль!", "Перемога", MessageBoxButton.OK, MessageBoxImage.Information);
                isGameOver = true;
            }
            else
            {
                btn.Style = (Style)Application.Current.Resources["MissButtonStyle"];
                btn.IsEnabled = false;
            }
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }
    }
}