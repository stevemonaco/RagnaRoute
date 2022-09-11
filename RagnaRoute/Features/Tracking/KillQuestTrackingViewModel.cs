using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NodaTime;
using RagnaRoute.Services;
using RagnaRoute.ViewExtenders;

namespace RagnaRoute.ViewModels;

public partial class KillQuestTrackingViewModel : TrackingGroupViewModel
{
    [ObservableProperty] private ObservableCollection<KillQuestViewModel> _quests = new();

    private readonly ISchedulerProvider _scheduler;
    private readonly CompletionService _completionService;

    private readonly IDisposable _cleanup;
    private bool _disposedValue;

    public KillQuestTrackingViewModel(CompletionService completionService)
    {
        _completionService = completionService;
    }

    public override void UpdateObjective()
    {
        foreach (var quest in Quests)
            quest.UpdateObjective();
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task CompleteObjectiveCommand(KillQuestViewModel viewModel)
    {
        var instant = SystemClock.Instance.GetCurrentInstant();
        viewModel.Objective.Next();
        viewModel.UpdateObjective();

        await _completionService.AddCompletion(Name, viewModel.Name, instant);
    }
}
