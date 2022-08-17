using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RagnaRoute.Services;
using System.Threading;
using System.Threading.Tasks;

namespace RagnaRoute.Views;
public partial class KillQuestTrackingView : UserControl
{
    private readonly IClipboardService _clipboardService;
    private readonly Grid _grid;
    private readonly Popup _popup;
    private CancellationTokenSource _popupCts;

    public KillQuestTrackingView()
    {
        _clipboardService = new ClipboardService();

        InitializeComponent();
        _grid = this.FindControl<Grid>("grid");
        _popup = this.FindControl<Popup>("popup");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void InfoButton_Clicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.DataContext is not string model)
            return;

        //var previousCts = _popupCts;
        //if (previousCts != null)
        //{
        //    previousCts.Cancel();
        //}

        //_popupCts = new CancellationTokenSource();

        var result = await _clipboardService.CopyTextAsync(model);

        if (result is true)
        {
            _popup.PlacementTarget = button;
            _popup.IsOpen = true;

            await Task.Delay(2000); //, _popupCts.Token);
            _popup.IsOpen = false;
            _popup.PlacementTarget = null;
        }
    }
}
