using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InfoSystem
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T property, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
            {
                return false;
            }

            property = value;

            OnPropertyChanged(name);

            return true;
        }
    }
}
