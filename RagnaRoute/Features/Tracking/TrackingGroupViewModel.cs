using CommunityToolkit.Mvvm.ComponentModel;

namespace RagnaRoute.ViewModels;
public abstract partial class TrackingGroupViewModel : ViewModelBase
{
    [ObservableProperty] private string _name;

    public abstract void UpdateObjective();
}
