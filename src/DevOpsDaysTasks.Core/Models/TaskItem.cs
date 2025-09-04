using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DevOpsDaysTasks.Core.Models;

public class TaskItem : INotifyPropertyChanged
{

    private string _title = string.Empty;
    private bool _isDone;

    public int Id { get; set; }

    public string Title
    {
        get => _title;
        set { if (_title != value) { _title = value; OnPropertyChanged(); } }
    }

    public bool IsDone
    {
        get => _isDone;
        set { if (_isDone != value) { _isDone = value; OnPropertyChanged(); } }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? prop = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}
