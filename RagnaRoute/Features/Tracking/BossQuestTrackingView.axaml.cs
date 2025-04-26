using Avalonia.Controls;
using Avalonia.Interactivity;
using RagnaRoute.Services;
using RagnaRoute.ViewModels;
using System.Threading.Tasks;
using RagnaRoute.ViewExtenders;
using RagnaRoute.Animations;
using Avalonia.Threading;

namespace RagnaRoute.Views;
public partial class BossQuestTrackingView : UserControl
{
    private readonly IClipboardService _clipboardService;
    private ResettableOperationRunner _copyPopupRunner = new();

    public BossQuestTrackingView()
    {
        InitializeComponent();

        _clipboardService = new ClipboardService();
    }

    private async void WarpLocation_Clicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: BossQuestViewModel { WarpLocation: string location } })
            return;

        await _copyPopupRunner.ExecuteOperationAsync(async token =>
        {
            try
            {
                var wasCopied = await _clipboardService.CopyTextAsync(location);

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
