using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using CoilWhineFix.Views;
using Microsoft.Win32;

namespace CoilWhineFix.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private int _delayMs;
    private bool _runAtStartup;
    private TaskType _selectedTask;

    public SettingsViewModel()
    {
        LoadSettings();
    }

    public bool RunAtStartup
    {
        get => _runAtStartup;
        set => SetProperty(ref _runAtStartup, value, nameof(RunAtStartup), UpdateStartupSetting);
    }

    public int DelayMs
    {
        get => _delayMs;
        set => SetProperty(ref _delayMs, value, nameof(DelayMs), () => UpdateSetting("DelayMs", _delayMs.ToString()));
    }

    public TaskType SelectedTask
    {
        get => _selectedTask;
        set => SetProperty(ref _selectedTask, value, nameof(SelectedTask),
            () => UpdateSetting("SelectedTask", _selectedTask.ToString()));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetProperty<T>(ref T field, T value, string propertyName, Action? afterChangeAction = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
        afterChangeAction?.Invoke();
    }

    private void UpdateStartupSetting()
    {
        ModifyRegistryStartup(RunAtStartup);
        UpdateSetting("RunAtStartup", RunAtStartup.ToString());
    }

    private static void ModifyRegistryStartup(bool addToStartup)
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        var mainModuleFileName = Process.GetCurrentProcess().MainModule?.FileName;
        if (mainModuleFileName == null) return;
        if (addToStartup)
            key?.SetValue("CoilWhineFix", mainModuleFileName);
        else
            key?.DeleteValue("CoilWhineFix", false);
    }

    private static void UpdateSetting(string key, string value)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        if (config.AppSettings.Settings[key] != null)
        {
            config.AppSettings.Settings[key].Value = value;
        }
        else
        {
            config.AppSettings.Settings.Add(key, value);
        }

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");
    }

    private void LoadSettings()
    {
        DelayMs = int.TryParse(ConfigurationManager.AppSettings["DelayMs"], out var delay) ? delay : 80;
        SelectedTask = Enum.TryParse(ConfigurationManager.AppSettings["SelectedTask"], out TaskType selectedTask)
            ? selectedTask
            : TaskType.GpuNopTask;
        RunAtStartup = bool.TryParse(ConfigurationManager.AppSettings["RunAtStartup"], out var runAtStartup) &&
                       runAtStartup;
    }
}