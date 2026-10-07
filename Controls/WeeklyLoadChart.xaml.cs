using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;

namespace Planner.Controls;

public sealed partial class WeeklyLoadChart : UserControl
{
    private static readonly (double X, string Label)[] BarSnapPoints = new[]
    {
        (31.0, "Monday: 6 tasks"),
        (69.0, "Tuesday: 7 tasks"),
        (107.0, "Wednesday (Today): 9 tasks (Peak load)"),
        (145.0, "Thursday: 5 tasks"),
        (183.0, "Friday: 3 tasks"),
        (221.0, "Saturday: 1 task"),
        (259.0, "Sunday: 1 task")
    };

    public WeeklyLoadChart()
    {
        InitializeComponent();
    }

    private void OnCanvasPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        Point pt = e.GetCurrentPoint(ChartCanvas).Position;
        double clampedX = Math.Clamp(pt.X, 10.0, 270.0);

        ScrubberLine.X1 = clampedX;
        ScrubberLine.X2 = clampedX;
        ScrubberLine.Visibility = Visibility.Visible;

        // Find closest bar snap point
        string bestLabel = BarSnapPoints[0].Label;
        double bestDist = double.MaxValue;
        foreach (var point in BarSnapPoints)
        {
            double dist = Math.Abs(point.X - clampedX);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestLabel = point.Label;
            }
        }

        TooltipText.Text = bestLabel;
        TooltipBadge.Visibility = Visibility.Visible;
    }

    private void OnCanvasPointerExited(object sender, PointerRoutedEventArgs e)
    {
        ScrubberLine.Visibility = Visibility.Collapsed;
        TooltipBadge.Visibility = Visibility.Collapsed;
    }
}
