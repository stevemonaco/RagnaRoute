using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using RagnaRoute.ViewModels;

namespace RagnaRoute.Views;
public partial class ShellView : Window
{
    private ShellViewModel? _viewModel;
    private DispatcherTimer _objectiveTimer;

    public ShellView()
    {
        InitializeComponent();
        _objectiveTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Render, ObjectiveTimer_Tick);
    }

    protected override async void OnDataContextChanged(EventArgs e)
    {
        _viewModel = DataContext as ShellViewModel;

        if (_viewModel is not null)
        {
            await _viewModel.InitializeTrackers();
            _viewModel.SelectedMenuItem = _viewModel.MenuItems.FirstOrDefault(x => x is BossQuestTrackingViewModel);
        }

        _objectiveTimer.Start();
        base.OnDataContextChanged(e);
    }

    private void ObjectiveTimer_Tick(object? sender, EventArgs e)
    {
        _viewModel?.UpdateObjectives();
    }
}
