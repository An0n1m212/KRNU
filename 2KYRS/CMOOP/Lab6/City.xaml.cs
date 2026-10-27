using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lab6
{
    public partial class City : Window
    {
        private int _balance = 0;

        private class BuildingItem
        {
            public string Name { get; set; }
            public int Price { get; set; }
            public Color Color { get; set; }

            public override string ToString() => $"{Name} ({Price} $)";
        }

        public City()
        {
            InitializeComponent();

            BuildingComboBox.Items.Add(new BuildingItem { Name = "Парк", Price = 50, Color = Colors.LightGreen });
            BuildingComboBox.Items.Add(new BuildingItem { Name = "Фабрика", Price = 100, Color = Colors.Orange });
            BuildingComboBox.Items.Add(new BuildingItem { Name = "Жилой дом", Price = 150, Color = Colors.LightSkyBlue });
            BuildingComboBox.SelectedIndex = 0;

            UpdateBalance();
        }

        private void Work_Click(object sender, RoutedEventArgs e)
        {
            _balance += 10;
            UpdateBalance();
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            if (BuildingComboBox.SelectedItem is BuildingItem building)
            {
                if (_balance >= building.Price)
                {
                    _balance -= building.Price;
                    UpdateBalance();
                    Border buildingTile = new Border
                    {
                        Width = 90,
                        Height = 90,
                        Margin = new Thickness(5),
                        Background = new SolidColorBrush(building.Color),
                        CornerRadius = new CornerRadius(8),
                        BorderBrush = Brushes.DarkSlateGray,
                        BorderThickness = new Thickness(2)
                    };

                    TextBlock text = new TextBlock
                    {
                        Text = building.Name,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontWeight = FontWeights.Bold,
                        TextWrapping = TextWrapping.Wrap
                    };

                    buildingTile.Child = text;
                    CityPanel.Children.Add(buildingTile);
                }
                else
                {
                    MessageBox.Show("Недостаточно средств для покупки!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void UpdateBalance()
        {
            BalanceTextBlock.Text = $"Баланс: {_balance} $";
        }
    }
}