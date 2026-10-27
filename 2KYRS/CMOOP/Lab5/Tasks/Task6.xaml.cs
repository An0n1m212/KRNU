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
    public partial class Task6 : Window
    {
        private byte _targetR, _targetG, _targetB;
        private bool _isCompleted = false;

        public Task6()
        {
            InitializeComponent();
            GenerateRecipe();
        }

        private void GenerateRecipe()
        {
            Random rnd = new Random();
            _targetR = (byte)rnd.Next(0, 256);
            _targetG = (byte)rnd.Next(0, 256);
            _targetB = (byte)rnd.Next(0, 256);
            _isCompleted = false;

            RecipeText.Text = $"Рецепт: R={_targetR}, G={_targetG}, B={_targetB}";
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CrystalRectangle == null) return;

            byte r = (byte)SliderR.Value;
            byte g = (byte)SliderG.Value;
            byte b = (byte)SliderB.Value;

            Color color = Color.FromRgb(r, g, b);
            CrystalRectangle.Fill = new SolidColorBrush(color);

            if (!_isCompleted && r == _targetR && g == _targetG && b == _targetB)
            {
                _isCompleted = true;
                MessageBox.Show("Зілля готове!", "Алхімія");
            }
        }

        private void NewRecipe_Click(object sender, RoutedEventArgs e)
        {
            GenerateRecipe();
        }
    }
}
