using Lab7.Task1.Models;
using Lab7.Task1.Services;
using System.Windows;

namespace Lab7.Task1.Views
{
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Заповніть обов'язкові поля (Логін, Email, Пароль)!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Паролі не збігаються!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtAge.Text, out int age) || age <= 0)
            {
                MessageBox.Show("Введіть коректний вік!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newUser = new User
            {
                Username = txtUsername.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Password = txtPassword.Password,
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                Age = age,
                Gender = rbMale.IsChecked == true ? "Чоловіча" : "Жіноча",
                Address = txtAddress.Text.Trim(),
                ZipCode = txtZipCode.Text.Trim(),
                About = txtAbout.Text.Trim()
            };

            if (UserService.SaveUser(newUser, out string error))
            {
                MessageBox.Show("Реєстрація успішна!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                var loginWindow = new Login();
                loginWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show(error, "Помилка реєстрації", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}