using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RagnaRoute.Services;

namespace RagnaRoute.Views;
public partial class ScheduledQuestTrackingView : UserControl
{
    private readonly IClipboardService _clipboardService;
    //private CancellationTokenSource _popupCts;

    public ScheduledQuestTrackingView()
    {
        _clipboardService = new ClipboardService();

        InitializeComponent();
    }

    private async void InfoButton_Clicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: string model })
        {
            var result = await _clipboardService.CopyTextAsync(model);

            if (result is true)
            {
                popup.PlacementTarget = (Button)sender;
                popup.IsOpen = true;

                await Task.Delay(1000);
                popup.IsOpen = false;
                popup.PlacementTarget = null;
            }
        }
    }
}
