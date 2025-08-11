using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSNetLib;

public class NotifyBaseModel : INotifyPropertyChanged
{
    public bool DataChanged = false;
    public event PropertyChangedEventHandler? PropertyChanged;
    // This method is called by the Set accessor of each property.  
    // The CallerMemberName attribute that is applied to the optional propertyName  
    // parameter causes the property name of the caller to be substituted as an argument.  
    protected bool SetProperty<T>(ref T backingStore,
                                  T value,
                                  [CallerMemberName] string propertyName = "",
                                  Action? onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;
        DataChanged = true;
        backingStore = value;
        onChanged?.Invoke();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
    public void NoticeProperty(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
