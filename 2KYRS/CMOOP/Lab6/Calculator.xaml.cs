using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Lab6
{
    public partial class Calculator : Window
    {
        private double _firstValue = 0;
        private string _operation = "";
        private bool _isNewEntry = true;

        public Calculator()
        {
            InitializeComponent();
        }

        private void Digit_Click(object sender, RoutedEventArgs e)
        {
            string digit = (sender as Button)?.Content.ToString();

            if (_isNewEntry || CurrentTextBox.Text == "0")
            {
                CurrentTextBox.Text = digit;
                _isNewEntry = false;
            }
            else
            {
                CurrentTextBox.Text += digit;
            }
        }

        private void Dot_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewEntry)
            {
                CurrentTextBox.Text = "0.";
                _isNewEntry = false;
            }
            else if (!CurrentTextBox.Text.Contains("."))
            {
                CurrentTextBox.Text += ".";
            }
        }

        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            _operation = (sender as Button)?.Content.ToString();
            _firstValue = double.Parse(CurrentTextBox.Text);
            HistoryTextBox.Text = $"{_firstValue} {_operation}";
            _isNewEntry = true;
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_operation)) return;

            double secondValue = double.Parse(CurrentTextBox.Text);
            double result = 0;

            switch (_operation)
            {
                case "+": result = _firstValue + secondValue; break;
                case "-": result = _firstValue - secondValue; break;
                case "*": result = _firstValue * secondValue; break;
                case "/":
                    if (secondValue == 0)
                    {
                        MessageBox.Show("Деление на ноль невозможно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    result = _firstValue / secondValue;
                    break;
            }

            HistoryTextBox.Text = $"{_firstValue} {_operation} {secondValue} =";
            CurrentTextBox.Text = result.ToString();
            _operation = "";
            _isNewEntry = true;
        }

        private void CE_Click(object sender, RoutedEventArgs e)
        {
            CurrentTextBox.Text = "0";
            _isNewEntry = true;
        }

        private void C_Click(object sender, RoutedEventArgs e)
        {
            CurrentTextBox.Text = "0";
            HistoryTextBox.Text = "";
            _firstValue = 0;
            _operation = "";
            _isNewEntry = true;
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewEntry) return;

            if (CurrentTextBox.Text.Length > 1)
                CurrentTextBox.Text = CurrentTextBox.Text.Substring(0, CurrentTextBox.Text.Length - 1);
            else
            {
                CurrentTextBox.Text = "0";
                _isNewEntry = true;
            }
        }
    }
}