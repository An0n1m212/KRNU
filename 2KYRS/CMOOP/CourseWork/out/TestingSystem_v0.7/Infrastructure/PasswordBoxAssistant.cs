using System.Windows;
using System.Windows.Controls;

namespace TestingSystem.Infrastructure;

/// <summary>
/// Attached-свойство для привязки PasswordBox к ViewModel без ссылки на контрол во ViewModel:
///   loc:PasswordBoxAssistant.BoundPassword="{Binding Password, Mode=TwoWay}"
/// </summary>
public static class PasswordBoxAssistant
{
    // Значение по умолчанию null (а не ""), чтобы первая привязка пустой строки вызвала callback и подписала обработчик
    public static readonly DependencyProperty BoundPasswordProperty = DependencyProperty.RegisterAttached(
        "BoundPassword", typeof(string), typeof(PasswordBoxAssistant),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundPasswordChanged));

    private static readonly DependencyProperty UpdatingProperty = DependencyProperty.RegisterAttached(
        "Updating", typeof(bool), typeof(PasswordBoxAssistant), new PropertyMetadata(false));

    public static string? GetBoundPassword(DependencyObject d) => (string?)d.GetValue(BoundPasswordProperty);
    public static void SetBoundPassword(DependencyObject d, string? value) => d.SetValue(BoundPasswordProperty, value);

    private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PasswordBox box) return;

        box.PasswordChanged -= OnPasswordChanged;
        if (!(bool)box.GetValue(UpdatingProperty))
            box.Password = (string?)e.NewValue ?? string.Empty;   // ViewModel -> View (например, очистка после ошибки)
        box.PasswordChanged += OnPasswordChanged;
    }

    private static void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        var box = (PasswordBox)sender;
        box.SetValue(UpdatingProperty, true);
        SetBoundPassword(box, box.Password);                       // View -> ViewModel
        box.SetValue(UpdatingProperty, false);
    }
}
