using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RagnaRoute.Views;
public partial class BossTrackingView : UserControl
{
    public BossTrackingView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
