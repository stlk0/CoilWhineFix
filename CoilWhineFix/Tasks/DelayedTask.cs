namespace CoilWhineFix.tasks;

public abstract class DelayedTask(int initialDelay) : IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    private readonly ManualResetEventSlim _pauseEvent = new(false);
    protected int DelayMs = initialDelay;
    private Task? _task;
    
    public void UpdateDelay(int newDelay)
    {
        DelayMs = newDelay;
    }

    public void InitializeTask()
    {
        if (_task == null || _task.IsCompleted)
        {
            _task = Task.Run(() => RunTask(_cts.Token));
        }
    }

    protected abstract void RunTask(CancellationToken cancellationToken);

    public void Pause()
    {
        _pauseEvent.Reset();
    }

    public void Resume()
    {
        _pauseEvent.Set();
    }

    protected void CheckPause()
    {
        _pauseEvent.Wait();
    }
    
    public void Dispose()
    {
        Resume();
        _cts.Cancel();
        _task?.Wait();
        _cts.Dispose();
        _pauseEvent.Dispose();
    }
}