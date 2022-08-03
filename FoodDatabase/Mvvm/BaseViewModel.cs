using System.ComponentModel;

namespace FoodDatabase.Mvvm
{
    public abstract class BaseViewModel : IViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual Action<Action> Dispatch { get; private set; }

        public abstract Task InitAsync();

        public void SetDispatcher(Action<Action> dispatch) => Dispatch = dispatch;

        public void RaisePropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));               
    }
}
