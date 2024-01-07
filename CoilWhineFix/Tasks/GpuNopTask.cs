using ILGPU;
using ILGPU.Runtime;

namespace CoilWhineFix.tasks;

public class GpuNopTask : DelayedTask
{
    private readonly Accelerator _accelerator;
    private readonly Context _context;

    public GpuNopTask(int initialDelay) : base(initialDelay)
    {
        _context = Context.CreateDefault();
        _accelerator = _context.GetPreferredDevice(false).CreateAccelerator(_context);
    }

    protected override void RunTask(CancellationToken cancellationToken)
    {
        using var deviceOutput = _accelerator.Allocate1D<int>(1);
        var loadedKernel = _accelerator.LoadAutoGroupedStreamKernel<Index1D, ArrayView<int>>(Kernel);
        
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                CheckPause();
                loadedKernel((int)deviceOutput.Length, deviceOutput.View);
                _accelerator.Synchronize();
                Task.Delay(DelayMs, cancellationToken).Wait(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation
        }
    }

    private static void Kernel(Index1D i, ArrayView<int> output)
    {
    }

    public new void Dispose()
    {
        base.Dispose();
        _accelerator.Dispose();
        _context.Dispose();
    }
}