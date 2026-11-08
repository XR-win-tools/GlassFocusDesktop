using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Media;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using GlassFocusDesktop.Models;

namespace GlassFocusDesktop.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly DispatcherTimer _clockTimer;
    private readonly DispatcherTimer _pomodoroTimer;

    private string _currentTime = "";
    private string _pomodoroTime = "25:00";
    private double _pomodoroProgress;
    private bool _isPomodoroRunning;
    private bool _isWorkPhase = true;
    private int _remainingSeconds = 25 * 60;
    private readonly int _workMinutes = 25;
    private readonly int _breakMinutes = 5;
    private string _newTodoText = "";
    private bool _isPinned;
    private bool _isCollapsed;

    public MainViewModel()
    {
        Todos = new ObservableCollection<TodoItem>
        {
            new() { Text = "打开 Visual Studio 2026" },
            new() { Text = "编写 Glass Focus 代码" },
            new() { Text = "享受液态玻璃" }
        };

        _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _clockTimer.Tick += (_, _) => CurrentTime = DateTime.Now.ToString("HH:mm:ss");
        _clockTimer.Start();
        CurrentTime = DateTime.Now.ToString("HH:mm:ss");

        _pomodoroTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _pomodoroTimer.Tick += PomodoroTick;
    }

    public ObservableCollection<TodoItem> Todos { get; }

    public string CurrentTime
    {
        get => _currentTime;
        set { _currentTime = value; OnPropertyChanged(); }
    }

    public string PomodoroTime
    {
        get => _pomodoroTime;
        set { _pomodoroTime = value; OnPropertyChanged(); }
    }

    public double PomodoroProgress
    {
        get => _pomodoroProgress;
        set { _pomodoroProgress = value; OnPropertyChanged(); }
    }

    public bool IsPomodoroRunning
    {
        get => _isPomodoroRunning;
        set { _isPomodoroRunning = value; OnPropertyChanged(); }
    }

    public bool IsWorkPhase
    {
        get => _isWorkPhase;
        set
        {
            _isWorkPhase = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PhaseText));
        }
    }

    public string PhaseText => IsWorkPhase ? "工作" : "休息";

    public string NewTodoText
    {
        get => _newTodoText;
        set { _newTodoText = value; OnPropertyChanged(); }
    }

    public bool IsPinned
    {
        get => _isPinned;
        set { _isPinned = value; OnPropertyChanged(); }
    }

    public bool IsCollapsed
    {
        get => _isCollapsed;
        set { _isCollapsed = value; OnPropertyChanged(); }
    }

    public void StartPausePomodoro()
    {
        if (IsPomodoroRunning)
        {
            _pomodoroTimer.Stop();
            IsPomodoroRunning = false;
        }
        else
        {
            _pomodoroTimer.Start();
            IsPomodoroRunning = true;
        }
    }

    public void ResetPomodoro()
    {
        _pomodoroTimer.Stop();
        IsPomodoroRunning = false;
        IsWorkPhase = true;
        _remainingSeconds = _workMinutes * 60;
        UpdatePomodoroDisplay();
    }

    private void PomodoroTick(object? sender, EventArgs e)
    {
        _remainingSeconds--;
        if (_remainingSeconds <= 0)
        {
            IsWorkPhase = !IsWorkPhase;
            _remainingSeconds = (IsWorkPhase ? _workMinutes : _breakMinutes) * 60;
            SystemSounds.Asterisk.Play();
        }
        UpdatePomodoroDisplay();
    }

    private void UpdatePomodoroDisplay()
    {
        var minutes = _remainingSeconds / 60;
        var seconds = _remainingSeconds % 60;
        PomodoroTime = $"{minutes:00}:{seconds:00}";
        var total = (IsWorkPhase ? _workMinutes : _breakMinutes) * 60;
        PomodoroProgress = 1.0 - (double)_remainingSeconds / total;
    }

    public void AddTodo()
    {
        if (!string.IsNullOrWhiteSpace(NewTodoText))
        {
            Todos.Add(new TodoItem { Text = NewTodoText.Trim() });
            NewTodoText = "";
        }
    }

    public void RemoveTodo(TodoItem item) => Todos.Remove(item);

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}