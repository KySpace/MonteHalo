using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;



namespace MonteHalo.Models
{
    public class NotificationBase : System.ComponentModel.INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetProperty<T>(ref T target, T value, [CallerMemberName] String propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(target, value))
                return false;
            target = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
        protected bool SetProperty<T>(T target, T value, Action doSet, [CallerMemberName] String propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(target, value))
                return false;
            doSet.Invoke();
            this.OnPropertyChanged(propertyName);
            return true;
        }

    }
    public class NotificationBase<T> : NotificationBase where T : class, new()
    {
        protected T This;
        public static implicit operator T(NotificationBase<T> thing) { return thing.This; }
        public NotificationBase(T thing = null)
        {
            This = (thing == null) ? new T() : thing;
        }
    }
}
