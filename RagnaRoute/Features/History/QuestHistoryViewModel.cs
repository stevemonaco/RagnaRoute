using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RagnaRoute.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagnaRoute.Features.History;

public partial class QuestHistoryViewModel : ObservableObject
{
    private readonly QuestService _questService;
    [ObservableProperty] private string _objectiveFamily;
    [ObservableProperty] private string _objectiveName;
    [ObservableProperty] private ObservableCollection<string> _completions = new();

    public QuestHistoryViewModel(QuestService questService)
    {
        _questService = questService;
    }

    [RelayCommand]
    public async Task UpdateHistory()
    {
        //if (string.IsNullOrEmpty(_objectiveFamily) || string.IsNullOrEmpty(_objectiveName))
        //    return;

        //var itemTask = await _questService.GetBossQuestCompletions(ObjectiveFamily, ObjectiveName);

        //Completions = new();
    }
}
