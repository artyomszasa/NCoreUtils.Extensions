using System.ComponentModel;

namespace NCoreUtils;

public abstract class NotifyPropertyBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new(propertyName));
}