using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>
/// Простой линейный график динамики успеваемости (0–100%). Рисуется на Canvas;
/// цвета подключены через SetResourceReference, поэтому следуют текущей теме без перерисовки.
/// </summary>
public class MiniLineChart : Canvas
{
    private IReadOnlyList<ProgressPoint> _points = Array.Empty<ProgressPoint>();

    public MiniLineChart() => SizeChanged += (_, _) => Redraw();

    public void SetData(IReadOnlyList<ProgressPoint> points)
    {
        _points = points;
        Redraw();
    }

    private void Label(string text, double x, double y)
    {
        var tb = new TextBlock { Text = text, FontSize = 10 };
        tb.SetResourceReference(TextBlock.ForegroundProperty, "SecondaryTextBrush");
        SetLeft(tb, x);
        SetTop(tb, y);
        Children.Add(tb);
    }

    private void Redraw()
    {
        Children.Clear();
        double w = ActualWidth, h = ActualHeight;
        if (w < 80 || h < 60) return;

        const double left = 34, bottom = 20, top = 8, right = 10;
        double plotW = w - left - right, plotH = h - top - bottom;

        // горизонтальные линии 0 / 50 / 100 %
        foreach (int pct in new[] { 0, 50, 100 })
        {
            double y = top + plotH * (1 - pct / 100.0);
            var grid = new Line { X1 = left, X2 = left + plotW, Y1 = y, Y2 = y, StrokeThickness = 1, Opacity = 0.5 };
            grid.SetResourceReference(Shape.StrokeProperty, "CardBorderBrush");
            Children.Add(grid);
            Label($"{pct}%", 2, y - 7);
        }

        if (_points.Count == 0) return;

        var coords = _points.Select((p, i) => new Point(
            left + (_points.Count == 1 ? plotW / 2 : plotW * i / (_points.Count - 1)),
            top + plotH * (1 - Math.Clamp(p.Percent, 0, 100) / 100.0))).ToList();

        var line = new Polyline { StrokeThickness = 2, Points = new PointCollection(coords) };
        line.SetResourceReference(Shape.StrokeProperty, "AccentBrush");
        Children.Add(line);

        for (int i = 0; i < coords.Count; i++)
        {
            var dot = new Ellipse { Width = 8, Height = 8, ToolTip = $"{_points[i].Date:dd.MM.yyyy}: {_points[i].Percent:0.#}%" };
            dot.SetResourceReference(Shape.FillProperty, "AccentBrush");
            SetLeft(dot, coords[i].X - 4);
            SetTop(dot, coords[i].Y - 4);
            Children.Add(dot);

            if (i == 0 || i == coords.Count - 1 || coords.Count <= 6)
                Label($"{_points[i].Date:dd.MM}", coords[i].X - 14, h - bottom + 3);
        }
    }
}
