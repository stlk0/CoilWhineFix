using System.Drawing;
using System.Runtime.InteropServices;

namespace CoilWhineFix.tasks;

public class DrawPixelTask(int initialDelay) : DelayedTask(initialDelay)
{
    [DllImport("User32.dll")]
    private static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("User32.dll")]
    private static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

    protected override void RunTask(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                CheckPause();
                DrawTransparentPixel();
                Task.Delay(DelayMs, cancellationToken).Wait(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation
        }
    }

    private static void DrawTransparentPixel()
    {
        var desktopPtr = GetDC(IntPtr.Zero);
        if (desktopPtr == IntPtr.Zero) return;

        try
        {
            using var graphics = Graphics.FromHdc(desktopPtr);
            using var brush = new SolidBrush(Color.FromArgb(0, 0, 0, 0));
            graphics.FillRectangle(brush, new Rectangle(0, 10, 1, 1));
        }
        finally
        {
            ReleaseDC(IntPtr.Zero, desktopPtr);
        }
    }
}