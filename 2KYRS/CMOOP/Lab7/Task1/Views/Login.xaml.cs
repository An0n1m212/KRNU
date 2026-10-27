using Lab7.Task1.Services;
using System.Windows;
using System.Windows.Controls;


namespace Lab7.Task1.Views
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (UserService.ValidateUser(login, password))
            {
                MessageBox.Show("Авторизація успішна! Ласкаво просимо.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Невірний логін або пароль!", "Помилка входу", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SocialLogin_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                MessageBox.Show($"Імітація входу через сервіс {btn.Content}! Авторизацію успішно виконано.",
                                "Соціальний вхід", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void OpenRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new Register();
            registerWindow.Show();
            this.Close();
        }
    }
}