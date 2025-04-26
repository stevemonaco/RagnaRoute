using Avalonia.Controls;
using Avalonia.Interactivity;
using RagnaRoute.Services;
using RagnaRoute.ViewModels;
using System.Threading.Tasks;
using RagnaRoute.ViewExtenders;
using RagnaRoute.Animations;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;

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
        if (sender is not Button { DataContext: BossQuestViewModel { WarpLocation: string location }, Content : FAPathIcon copyIcon })
            return;

        await _copyPopupRunner.ExecuteOperationAsync(async token =>
        {
            try
            {
                var wasCopied = await _clipboardService.CopyTextAsync(location);

                if (!wasCopied)
                    return;

                popup.PlacementTarget = (Button)sender;
                popup.Opacity = 1d;
                popup.IsOpen = true;

                var popupAnimation = PrebuiltAnimations.PopupFade.RunAsync(popup, token);
                var iconAnimation = PrebuiltAnimations.CopyIconColorShift.RunAsync(copyIcon, token);
                await Task.WhenAll(popupAnimation, iconAnimation);
            }
            finally
            {
                popup.IsOpen = false;
                popup.PlacementTarget = null;
            }
        });
    }
}
