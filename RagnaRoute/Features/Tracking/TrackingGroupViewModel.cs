using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Reactive.Disposables;

namespace RagnaRoute.ViewModels;
public abstract partial class TrackingGroupViewModel : PageViewModel, IDisposable
{
    [ObservableProperty] private string _name = null!;

    protected CompositeDisposable _cleanup = new();
    private bool _disposedValue;

    public abstract void UpdateObjective();

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _cleanup.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
