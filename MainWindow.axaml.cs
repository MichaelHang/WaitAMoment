using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.ComponentModel;

namespace WaitAMoment;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private double _progressValue;
    private string _centerText = "";
    private bool _isToggleChecked = false;
    private bool _isStartEnabled = true;
    private bool _isResetEnabled = false;
    private bool _isTimePickerEnabled = true;
    private TimeSpan? _selectedTime = new TimeSpan(10, 0, 0);
    private DispatcherTimer? _timer;
    private int _totalSeconds = 600;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        _progressValue = 100;
        UpdateCenterText();

        // Initialize timer
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        _timer.Tick += Timer_Tick;
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    public double ProgressValue
    {
        get => _progressValue;
        set
        {
            if (_progressValue != value)
            {
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
                UpdateCenterText();
            }
        }
    }

    public string CenterText
    {
        get => _centerText;
        set
        {
            if (_centerText != value)
            {
                _centerText = value;
                OnPropertyChanged(nameof(CenterText));
            }
        }
    }

    public bool IsToggleChecked
    {
        get => _isToggleChecked;
        set
        {
            if (_isToggleChecked != value)
            {
                _isToggleChecked = value;
                OnPropertyChanged(nameof(IsToggleChecked));
                SwitchTheme(value);
            }
        }
    }

    public bool IsStartEnabled
    {
        get => _isStartEnabled;
        set
        {
            if (_isStartEnabled != value)
            {
                _isStartEnabled = value;
                OnPropertyChanged(nameof(IsStartEnabled));
            }
        }
    }

    public bool IsResetEnabled
    {
        get => _isResetEnabled;
        set
        {
            if (_isResetEnabled != value)
            {
                _isResetEnabled = value;
                OnPropertyChanged(nameof(IsResetEnabled));
            }
        }
    }

    public bool IsTimePickerEnabled
    {
        get => _isTimePickerEnabled;
        set
        {
            if (_isTimePickerEnabled != value)
            {
                _isTimePickerEnabled = value;
                OnPropertyChanged(nameof(IsTimePickerEnabled));
            }
        }
    }

    public TimeSpan? SelectedTime
    {
        get => _selectedTime;
        set
        {
            if (_selectedTime != value)
            {
                _selectedTime = value;
                OnPropertyChanged(nameof(SelectedTime));
                if (_selectedTime.HasValue)
                {
                    // Hours from picker = minutes for timer
                    // Minutes from picker = seconds for timer
                    int minutes = _selectedTime.Value.Hours;
                    int seconds = _selectedTime.Value.Minutes;
                    _totalSeconds = (minutes * 60) + seconds;

                    if (_timer != null && !_timer.IsEnabled)
                    {
                        ProgressValue = 100;
                        UpdateCenterText();
                    }
                }
            }
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void StartButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_timer != null && !_timer.IsEnabled)
        {
            _timer.Start();
            IsStartEnabled = false;
            IsResetEnabled = true;
            IsTimePickerEnabled = false;
        }
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_timer != null)
        {
            _timer.Stop();
            ProgressValue = 100;
            IsStartEnabled = true;
            IsResetEnabled = false;
            IsTimePickerEnabled = true;
        }
    }

    private void ExitButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void SwitchTheme(bool isLightTheme)
    {
        var app = Avalonia.Application.Current;
        if (app != null)
        {
            app.RequestedThemeVariant = isLightTheme
                ? Avalonia.Styling.ThemeVariant.Light
                : Avalonia.Styling.ThemeVariant.Dark;
        }
    }

    private void UpdateCenterText()
    {
        double secondsRemaining = (ProgressValue / 100.0) * _totalSeconds;
        int totalSeconds = (int)Math.Ceiling(secondsRemaining);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        CenterText = $"{minutes:D2}:{seconds:D2}";
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        double step = 100.0 / (_totalSeconds * 10.0);

        if (ProgressValue > 0)
        {
            ProgressValue -= step;
        }
        else
        {
            _timer?.Stop();
        }
    }
}
