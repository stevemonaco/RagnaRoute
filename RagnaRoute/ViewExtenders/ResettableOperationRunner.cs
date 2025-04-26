using System;
using System.Threading;
using System.Threading.Tasks;

namespace RagnaRoute.ViewExtenders;

/// <summary>
/// Restricts operations to run one at a time
/// Cancels current operations in favor of starting a new one
/// </summary>
public class ResettableOperationRunner
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private CancellationTokenSource? _currentCts;

    public async Task ExecuteOperationAsync(Func<CancellationToken, Task> operationFactory)
    {
        _currentCts?.Cancel();
        _currentCts = new CancellationTokenSource();

        try
        {
            await _semaphore.WaitAsync(_currentCts.Token);
            try
            {
                await operationFactory(_currentCts.Token);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}