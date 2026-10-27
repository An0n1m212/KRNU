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
    /// <summary>
    /// Логика взаимодействия для Task7.xaml
    /// </summary>
    public partial class Task7 : Window
    {
        private int _secretCode;
        private int _attemptsLeft;
        private readonly Random _rnd = new Random();

        public Task7()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void StartNewGame()
        {
            _secretCode = _rnd.Next(1000, 10000);
            _attemptsLeft = 5;
            AttemptsText.Text = $"Залишилось спроб: {_attemptsLeft}";
            HistoryBox.Text = "=== Нова гра розпочата ===\n";
            CodeInput.IsEnabled = true;
            UpdateXRayHint();
        }

        private void CheckCode_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(CodeInput.Text, out int userCode) || CodeInput.Text.Length != 4)
            {
                MessageBox.Show("Будь ласка, введіть коректне 4-значне число!");
                return;
            }

            _attemptsLeft--;

            if (userCode == _secretCode)
            {
                HistoryBox.AppendText($"Спроба: {userCode} — СЕЙФ ВЗЛАМАНО! Перемога!\n");
                CodeInput.IsEnabled = false;
                MessageBox.Show("Вітаємо! Сейф відкрито!");
                return;
            }

            string hint = userCode < _secretCode ? "Код більший." : "Код менший.";
            HistoryBox.AppendText($"Спроба ({5 - _attemptsLeft}): {userCode} — Сейф заблоковано. {hint}\n");

            if (_attemptsLeft <= 0)
            {
                HistoryBox.AppendText($"ГРА ЗАКІНЧЕНА! Правильний код: {_secretCode}\n");
                CodeInput.IsEnabled = false;
                MessageBox.Show($"Ви програли! Правильний код був: {_secretCode}");
            }
            else
            {
                AttemptsText.Text = $"Залишилось спроб: {_attemptsLeft}";
            }
        }

        private void XRay_Changed(object sender, RoutedEventArgs e)
        {
            UpdateXRayHint();
        }

        private void UpdateXRayHint()
        {
            if (XRayCheckBox.IsChecked == true)
            {
                string codeStr = _secretCode.ToString("D4");
                XRayHint.Text = $"Підказка рентгена: Перша цифра — {codeStr[0]}";
            }
            else
            {
                XRayHint.Text = string.Empty;
            }
        }
    }
}
