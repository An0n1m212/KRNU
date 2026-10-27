using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace TestingSystem.Infrastructure;

/// <summary>
/// Мінімальний рендерер Markdown у FlowDocument: заголовки (#, ##, ###), списки (- та 1.), **жирний**, *курсив*,
/// `код` і блоки ```код```. Свідомо НЕ підтримує HTML та посилання: текст від AI/автора лише відображається,
/// ніколи не виконується і не відкриває зовнішніх ресурсів.
/// </summary>
public static class MarkdownRenderer
{
    private static readonly Regex Inline = new(@"\*\*(?<b>.+?)\*\*|\*(?<i>[^\s*][^*]*?)\*|`(?<c>[^`]+)`", RegexOptions.Compiled);
    private static readonly Regex Heading = new(@"^(?<h>#{1,3})\s+(?<t>.+)$", RegexOptions.Compiled);
    private static readonly Regex Bullet = new(@"^\s*[-*+]\s+(?<t>.+)$", RegexOptions.Compiled);
    private static readonly Regex Numbered = new(@"^\s*(?<n>\d+)[.)]\s+(?<t>.+)$", RegexOptions.Compiled);

    public static FlowDocument Render(string? markdown)
    {
        var doc = new FlowDocument { PagePadding = new Thickness(8), FontSize = 15, FontFamily = new FontFamily("Segoe UI") };
        doc.SetResourceReference(TextElement.ForegroundProperty, "TextBrush");   // колір тексту береться з поточної теми

        var paragraph = new StringBuilder();
        var code = new StringBuilder();
        bool inCode = false;

        void FlushParagraph()
        {
            if (paragraph.Length == 0) return;
            doc.Blocks.Add(MakeParagraph(paragraph.ToString().Trim(), new Thickness(0, 0, 0, 10)));
            paragraph.Clear();
        }

        void FlushCode()
        {
            var p = new Paragraph(new Run(code.ToString().TrimEnd()))
            {
                FontFamily = new FontFamily("Consolas"), Padding = new Thickness(10), Margin = new Thickness(0, 0, 0, 10)
            };
            p.SetResourceReference(TextElement.BackgroundProperty, "HeaderBackgroundBrush");
            doc.Blocks.Add(p);
            code.Clear();
        }

        foreach (var rawLine in (markdown ?? string.Empty).Replace("\r\n", "\n").Split('\n'))
        {
            string line = rawLine.TrimEnd();

            if (line.TrimStart().StartsWith("```"))
            {
                if (inCode) { FlushCode(); inCode = false; }
                else { FlushParagraph(); inCode = true; }
                continue;
            }
            if (inCode) { code.AppendLine(line); continue; }

            if (line.Trim().Length == 0) { FlushParagraph(); continue; }

            var heading = Heading.Match(line);
            if (heading.Success)
            {
                FlushParagraph();
                int level = heading.Groups["h"].Length;
                var p = MakeParagraph(heading.Groups["t"].Value.Trim(), new Thickness(0, level == 1 ? 6 : 10, 0, 6));
                p.FontSize = level switch { 1 => 26, 2 => 21, _ => 17 };
                p.FontWeight = FontWeights.Bold;
                doc.Blocks.Add(p);
                continue;
            }

            var bullet = Bullet.Match(line);
            if (bullet.Success)
            {
                FlushParagraph();
                doc.Blocks.Add(MakeParagraph("•  " + bullet.Groups["t"].Value, new Thickness(18, 0, 0, 3)));
                continue;
            }

            var numbered = Numbered.Match(line);
            if (numbered.Success)
            {
                FlushParagraph();
                doc.Blocks.Add(MakeParagraph($"{numbered.Groups["n"].Value}.  {numbered.Groups["t"].Value}", new Thickness(18, 0, 0, 3)));
                continue;
            }

            if (paragraph.Length > 0) paragraph.Append(' ');
            paragraph.Append(line.Trim());
        }

        FlushParagraph();
        if (inCode) FlushCode();
        return doc;
    }

    private static Paragraph MakeParagraph(string text, Thickness margin)
    {
        var p = new Paragraph { Margin = margin };
        int pos = 0;
        foreach (Match m in Inline.Matches(text))
        {
            if (m.Index > pos) p.Inlines.Add(new Run(text[pos..m.Index]));

            if (m.Groups["b"].Success) p.Inlines.Add(new Bold(new Run(m.Groups["b"].Value)));
            else if (m.Groups["i"].Success) p.Inlines.Add(new Italic(new Run(m.Groups["i"].Value)));
            else
            {
                var run = new Run(m.Groups["c"].Value) { FontFamily = new FontFamily("Consolas") };
                run.SetResourceReference(TextElement.BackgroundProperty, "HeaderBackgroundBrush");
                p.Inlines.Add(run);
            }
            pos = m.Index + m.Length;
        }
        if (pos < text.Length) p.Inlines.Add(new Run(text[pos..]));
        return p;
    }
}
