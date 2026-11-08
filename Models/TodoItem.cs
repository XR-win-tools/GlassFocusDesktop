using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GlassFocusDesktop.Models;

public class TodoItem : INotifyPropertyChanged
{
    private string _text = "";
    private bool _isDone;

    public string Text
    {
        get => _text;
        set { _text = value; OnPropertyChanged(); }
    }

    public bool IsDone
    {
        get => _isDone;
        set { _isDone = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}