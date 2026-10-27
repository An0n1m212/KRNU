using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Lab6
{
    public partial class BestOil : Window
    {  
        private Dictionary<string, double> _fuelPrices = new Dictionary<string, double>
        {
            { "А-95+", 62.50 },
            { "А-95", 58.00 },
            { "А-92", 55.00 },
            { "ДП", 54.00 }
        };

            private double _dailyTotalIncome = 0;

            public BestOil()
            {
                InitializeComponent();
                foreach (var fuel in _fuelPrices.Keys)
                {
                    FuelComboBox.Items.Add(fuel);
                }
                FuelComboBox.SelectedIndex = 0;
            }

            private void FuelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                if (FuelComboBox.SelectedItem == null) return;
                string selected = FuelComboBox.SelectedItem.ToString();
                FuelPriceTextBox.Text = _fuelPrices[selected].ToString("F2");
                CalculateFuel();
            }

            private void Mode_Checked(object sender, RoutedEventArgs e)
            {
                if (FuelResultGroupBox == null) return;

                if (LitersRadioButton.IsChecked == true)
                {
                    FuelResultGroupBox.Header = "До оплати:";
                    FuelUnitTextBlock.Text = " грн.";
                }
                else
                {
                    FuelResultGroupBox.Header = "До видачі:";
                    FuelUnitTextBlock.Text = " л.";
                }
                CalculateFuel();
            }

            private void FuelInput_TextChanged(object sender, TextChangedEventArgs e)
            {
                CalculateFuel();
            }

            private void CalculateFuel()
            {
                if (FuelPriceTextBox == null || FuelQuantityTextBox == null) return;

                if (!double.TryParse(FuelPriceTextBox.Text, out double price) || price <= 0) return;
                if (!double.TryParse(FuelQuantityTextBox.Text, out double inputVal))
                {
                    FuelResultTextBlock.Text = "0,00";
                    return;
                }

                if (LitersRadioButton.IsChecked == true)
                {
                    double cost = inputVal * price;
                    FuelResultTextBlock.Text = cost.ToString("F2");
                }
                else
                {
                    double liters = inputVal / price;
                    FuelResultTextBlock.Text = liters.ToString("F2");
                }
            }

            private void CafeCheckBox_Changed(object sender, RoutedEventArgs e)
            {
                HotDogCount.IsEnabled = HotDogCheckBox.IsChecked == true;
                BurgerCount.IsEnabled = BurgerCheckBox.IsChecked == true;
                FriesCount.IsEnabled = FriesCheckBox.IsChecked == true;
                ColaCount.IsEnabled = ColaCheckBox.IsChecked == true;

                CalculateCafe();
            }

            private void CafeCount_TextChanged(object sender, TextChangedEventArgs e)
            {
                CalculateCafe();
            }

        private double CalculateCafe()
        {
            double sum = 0;

            if (HotDogCheckBox?.IsChecked == true &&
                double.TryParse(HotDogCount?.Text, out double c1) &&
                double.TryParse(HotDogPrice?.Text, out double p1))
            {
                sum += c1 * p1;
            }

            if (BurgerCheckBox?.IsChecked == true &&
                double.TryParse(BurgerCount?.Text, out double c2) &&
                double.TryParse(BurgerPrice?.Text, out double p2))
            {
                sum += c2 * p2;
            }

            if (FriesCheckBox?.IsChecked == true &&
                double.TryParse(FriesCount?.Text, out double c3) &&
                double.TryParse(FriesPrice?.Text, out double p3))
            {
                sum += c3 * p3;
            }

            if (ColaCheckBox?.IsChecked == true &&
                double.TryParse(ColaCount?.Text, out double c4) &&
                double.TryParse(ColaPrice?.Text, out double p4))
            {
                sum += c4 * p4;
            }

            if (CafeResultTextBlock != null)
            {
                CafeResultTextBlock.Text = sum.ToString("F2");
            }

            return sum;
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
            {
                double fuelSum = 0;
                if (LitersRadioButton.IsChecked == true)
                {
                    double.TryParse(FuelResultTextBlock.Text, out fuelSum);
                }
                else
                {
                    double.TryParse(FuelQuantityTextBox.Text, out fuelSum);
                }

                double cafeSum = CalculateCafe();
                double total = fuelSum + cafeSum;

                TotalResultTextBlock.Text = total.ToString("F2");
                _dailyTotalIncome += total;
            }

            private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
            {
                MessageBox.Show($"Загальна сума виручки за цей день склала: {_dailyTotalIncome:F2} грн.", "Підсумок зміни BestOil", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
