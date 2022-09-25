using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RagnaRoute.ViewModels;
using System;

namespace RagnaRoute.Views;
public partial class QuestHistoryView : UserControl
{
    //private QuestHistoryViewModel? _viewModel;

    public QuestHistoryView()
    {
        InitializeComponent();
    }

    //protected async override void OnDataContextChanged(EventArgs e)
    //{
    //    base.OnDataContextChanged(e);
    //    //_viewModel = DataContext as QuestHistoryViewModel;

    //    //if (_viewModel is not null)
    //    //{
    //    //    await _viewModel.Initialize();
    //    //}
    //}
}
