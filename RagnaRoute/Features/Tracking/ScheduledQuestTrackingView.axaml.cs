using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RagnaRoute.Animations;
using RagnaRoute.Services;
using RagnaRoute.ViewExtenders;
using RagnaRoute.ViewModels;

namespace RagnaRoute.Views;
public partial class ScheduledQuestTrackingView : UserControl
{
    private readonly IClipboardService _clipboardService;
    private ResettableOperationRunner _copyPopupRunner = new();

    public ScheduledQuestTrackingView()
    {
        _clipboardService = new ClipboardService();

        InitializeComponent();
    }

    private async void InfoButton_Clicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: string model })
            return;

        await _copyPopupRunner.ExecuteOperationAsync(async token =>
        {
            try
            {
                var wasCopied = await _clipboardService.CopyTextAsync(model);

                if (!wasCopied)
                    return;

                popup.PlacementTarget = (Button)sender;
                popup.IsOpen = true;

                await PrebuiltAnimations.PopupFade.RunAsync(popup, token);
            }
            finally
            {
                popup.IsOpen = false;
                popup.PlacementTarget = null;
            }
        });
    }
}
