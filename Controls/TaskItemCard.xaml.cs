using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Planner.Models;

namespace Planner.Controls;

public sealed partial class TaskItemCard : UserControl
{
    public static readonly DependencyProperty TaskItemProperty =
        DependencyProperty.Register(
            nameof(TaskItem),
            typeof(PlannerTask),
            typeof(TaskItemCard),
            new PropertyMetadata(null));

    public PlannerTask TaskItem
    {
        get => (PlannerTask)GetValue(TaskItemProperty);
        set => SetValue(TaskItemProperty, value);
    }

    public event EventHandler<PlannerTask>? TaskToggleRequested;
    public event EventHandler<PlannerTask>? TaskDeleteRequested;
    public event EventHandler<PlannerTask>? SubTaskAddRequested;

    public TaskItemCard()
    {
        InitializeComponent();
    }

    private void OnCheckClicked(object sender, RoutedEventArgs e)
    {
        if (TaskItem != null)
        {
            TaskToggleRequested?.Invoke(this, TaskItem);
        }
    }

    private void OnDeleteClicked(object sender, RoutedEventArgs e)
    {
        if (TaskItem != null)
        {
            TaskDeleteRequested?.Invoke(this, TaskItem);
        }
    }

    private void OnRowTapped(object sender, TappedRoutedEventArgs e)
    {
        if (TaskItem != null)
        {
            TaskItem.IsExpanded = !TaskItem.IsExpanded;
        }
    }

    private void OnAddSubTaskClicked(object sender, RoutedEventArgs e)
    {
        if (TaskItem != null)
        {
            SubTaskAddRequested?.Invoke(this, TaskItem);
        }
    }

    private Brush GetCheckBackground(bool isCompleted)
    {
        if (isCompleted)
        {
            return ActualTheme == ElementTheme.Dark
                ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x60, 0xCD, 0xFF))
                : new SolidColorBrush(ColorHelper.FromArgb(255, 0x00, 0x5F, 0xB8));
        }
        return new SolidColorBrush(Colors.Transparent);
    }

    private Brush GetCheckBorder(bool isCompleted)
    {
        if (isCompleted)
        {
            return ActualTheme == ElementTheme.Dark
                ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x60, 0xCD, 0xFF))
                : new SolidColorBrush(ColorHelper.FromArgb(255, 0x00, 0x5F, 0xB8));
        }
        return ActualTheme == ElementTheme.Dark
            ? new SolidColorBrush(ColorHelper.FromArgb(255, 0x5A, 0x67, 0x7D))
            : new SolidColorBrush(ColorHelper.FromArgb(255, 0x8A, 0x8A, 0x8A));
    }

    private double GetTitleOpacity(bool isCompleted) => isCompleted ? 0.55 : 1.0;
}
