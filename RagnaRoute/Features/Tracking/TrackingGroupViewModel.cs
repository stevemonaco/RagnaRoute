using ReactiveUI;

namespace RagnaRoute.ViewModels;
public abstract class TrackingGroupViewModel : ViewModelBase
{
    private string _name;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public abstract void UpdateObjective();
}
