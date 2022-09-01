using CommunityToolkit.Mvvm.ComponentModel;

namespace RagnaRoute.ViewModels;
public abstract partial class TrackingGroupViewModel : ViewModelBase, INavigationChild
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _displayName;

    public abstract void UpdateObjective();
}
