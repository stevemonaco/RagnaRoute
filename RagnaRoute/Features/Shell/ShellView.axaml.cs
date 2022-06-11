using Avalonia.Controls;
using Avalonia.Threading;
using RagnaRoute.Data;
using RagnaRoute.Services;
using RagnaRoute.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RagnaRoute.Views;
public partial class ShellView : Window
{
    private ShellViewModel _viewModel;
    private DispatcherTimer _objectiveTimer;

    public ShellView()
    {
        InitializeComponent();

        _objectiveTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, ObjectiveTimer_Tick);
        _objectiveTimer.Start();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        _viewModel = DataContext as ShellViewModel;
    }

    private async void OnInitialized(object? sender, EventArgs e)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)}
        };

        var questsContent = await File.ReadAllTextAsync(@"_objectives\quests.json");
        var quests = JsonSerializer.Deserialize<List<QuestHeaderModel>>(questsContent, jsonOptions);

        if (quests is null)
            return;

        foreach (var quest in quests)
        {
            var questContent = await File.ReadAllTextAsync(@$"_objectives\{quest.FileName}");

            if (quest.Kind == QuestKind.Boss)
            {

            }
            else if (quest.Kind == QuestKind.Kill)
            {
                var killQuests = JsonSerializer.Deserialize<List<KillQuestModel>>(questContent, jsonOptions);

                if (killQuests is null)
                    continue;

                var trackingVm = new KillQuestTrackingViewModel()
                {
                    Name = quest.Name,
                    Quests = new(killQuests.Select(x => x.ToViewModel()))
                };

                _viewModel.Trackers.Add(trackingVm);
            }
        }
    }

    private void ObjectiveTimer_Tick(object? sender, EventArgs e)
    {
        _viewModel.UpdateObjectives();
    }
}
