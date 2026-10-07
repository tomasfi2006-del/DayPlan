using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Planner.Models;
using Windows.Foundation;

namespace Planner.Controls;

public sealed partial class WeeklyLoadChart : UserControl
{
    public static readonly DependencyProperty WeeklyLoadsProperty =
        DependencyProperty.Register(
            nameof(WeeklyLoads),
            typeof(IEnumerable<WeeklyDayLoad>),
            typeof(WeeklyLoadChart),
            new PropertyMetadata(null, OnWeeklyLoadsChanged));

    public IEnumerable<WeeklyDayLoad>? WeeklyLoads
    {
        get => (IEnumerable<WeeklyDayLoad>?)GetValue(WeeklyLoadsProperty);
        set => SetValue(WeeklyLoadsProperty, value);
    }

    private readonly List<(double X, string Label)> _dynamicSnapPoints = new();

    public WeeklyLoadChart()
    {
        InitializeComponent();
    }

    private static void OnWeeklyLoadsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WeeklyLoadChart chart)
        {
            if (e.OldValue is INotifyCollectionChanged oldList)
            {
                oldList.CollectionChanged -= chart.OnWeeklyLoadsCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newList)
            {
                newList.CollectionChanged += chart.OnWeeklyLoadsCollectionChanged;
            }

            chart.RefreshChart();
        }
    }

    private void OnWeeklyLoadsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshChart();
    }

    private void RefreshChart()
    {
        if (WeeklyLoads == null) return;
        var loads = WeeklyLoads.ToList();
        if (loads.Count < 7) return;

        Rectangle[] bars = [Bar0, Bar1, Bar2, Bar3, Bar4, Bar5, Bar6];
        double[] xPositions = [31.0, 69.0, 107.0, 145.0, 183.0, 221.0, 259.0];

        _dynamicSnapPoints.Clear();

        for (int i = 0; i < 7; i++)
        {
            var load = loads[i];
            var bar = bars[i];
            double height = Math.Max(4, Math.Min(58, load.TaskCount == 0 ? 4 : 4 + load.TaskCount * 6.0));
            bar.Height = height;
            Canvas.SetTop(bar, 70 - height);

            string dayLabel = load.IsToday
                ? $"{load.DayName} (Today): {load.TaskCount} {(load.TaskCount == 1 ? "task" : "tasks")}"
                : $"{load.DayName}: {load.TaskCount} {(load.TaskCount == 1 ? "task" : "tasks")}";

            _dynamicSnapPoints.Add((xPositions[i], dayLabel));
        }
    }

    private void OnCanvasPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        Point pt = e.GetCurrentPoint(ChartCanvas).Position;
        double clampedX = Math.Clamp(pt.X, 10.0, 270.0);

        ScrubberLine.X1 = clampedX;
        ScrubberLine.X2 = clampedX;
        ScrubberLine.Visibility = Visibility.Visible;

        if (_dynamicSnapPoints.Count == 0)
        {
            RefreshChart();
        }

        if (_dynamicSnapPoints.Count > 0)
        {
            string bestLabel = _dynamicSnapPoints[0].Label;
            double bestDist = double.MaxValue;
            foreach (var point in _dynamicSnapPoints)
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
    }

    private void OnCanvasPointerExited(object sender, PointerRoutedEventArgs e)
    {
        ScrubberLine.Visibility = Visibility.Collapsed;
        TooltipBadge.Visibility = Visibility.Collapsed;
    }
}
