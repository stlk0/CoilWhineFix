using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using CoilWhineFix.tasks;
using CoilWhineFix.ViewModels;

namespace CoilWhineFix.Views;

public enum TaskType
{
    GpuNopTask,
    DrawPixelTask
}

public partial class MainWindow
{
    private readonly DelayedTask _drawPixelTask;
    private readonly GpuNopTask _gpuNopTask;

    public MainWindow()
    {
        InitializeComponent();
        var settings = new SettingsViewModel();
        DataContext = settings;

        SetupEventHandlers();

        _gpuNopTask = new GpuNopTask(settings.DelayMs);
        _drawPixelTask = new DrawPixelTask(settings.DelayMs);
        _gpuNopTask.InitializeTask();
        _drawPixelTask.InitializeTask();

        LoadSelectedTaskSetting();
    }

    private void SetupEventHandlers()
    {
        DelayMsTextBox.TextChanged += DelayMsTextBox_TextChanged;
        RadioButtonGpuNopTask.Checked += (_, _) => UpdateTaskSelection(TaskType.GpuNopTask);
        RadioButtonDrawPixelTask.Checked += (_, _) => UpdateTaskSelection(TaskType.DrawPixelTask);
    }

    private void Window_StateChanged(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            (Application.Current as App)?.MinimizeToSystemTray();
        }
    }

    private void MinimizeToTray_Click(object sender, RoutedEventArgs e)
    {
        (Application.Current as App)?.MinimizeToSystemTray();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _gpuNopTask.Dispose();
        _drawPixelTask.Dispose();
    }

    private void DelayMsTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (int.TryParse(DelayMsTextBox.Text, out var newDelay))
        {
            _gpuNopTask.UpdateDelay(newDelay);
            _drawPixelTask.UpdateDelay(newDelay);
        }
        else
        {
            MessageBox.Show("Invalid delay value.");
        }
    }

    private void LoadSelectedTaskSetting()
    {
        var selectedTask = Enum.TryParse(ConfigurationManager.AppSettings["SelectedTask"], out TaskType task) ? task : TaskType.GpuNopTask;
        UpdateTaskSelection(selectedTask);
    }

    private void UpdateTaskSelection(TaskType selectedTask)
    {
        RadioButtonGpuNopTask.IsChecked = selectedTask == TaskType.GpuNopTask;
        RadioButtonDrawPixelTask.IsChecked = selectedTask == TaskType.DrawPixelTask;

        if (selectedTask == TaskType.GpuNopTask)
        {
            _drawPixelTask.Pause();
            _gpuNopTask.Resume();
        }
        else
        {
            _gpuNopTask.Pause();
            _drawPixelTask.Resume();
        }
    }
}