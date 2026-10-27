using System.Windows;
using System.Windows.Controls;

namespace TestingSystem.Infrastructure;

/// <summary>
/// Attached-властивість для MVVM: loc:MarkdownBehavior.Markdown="{Binding Content}" на FlowDocumentScrollViewer.
/// ViewModel зберігає лише рядок Markdown і нічого не знає про FlowDocument.
/// </summary>
public static class MarkdownBehavior
{
    public static readonly DependencyProperty MarkdownProperty = DependencyProperty.RegisterAttached(
        "Markdown", typeof(string), typeof(MarkdownBehavior), new PropertyMetadata(null, OnMarkdownChanged));

    public static string? GetMarkdown(DependencyObject d) => (string?)d.GetValue(MarkdownProperty);
    public static void SetMarkdown(DependencyObject d, string? value) => d.SetValue(MarkdownProperty, value);

    private static void OnMarkdownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FlowDocumentScrollViewer viewer)
            viewer.Document = MarkdownRenderer.Render(e.NewValue as string);
    }
}
