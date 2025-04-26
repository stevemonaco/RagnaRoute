using CommunityToolkit.Mvvm.ComponentModel;

namespace RagnaRoute.ViewModels;
public abstract partial class PageViewModel : ViewModelBase, INavigationChild
{
    [ObservableProperty] private string? _displayName = null!;
}
