namespace FoodDatabase.Mvvm
{
    public interface IViewModel
    {
        void Init();
        Action<Action> Dispatch { get; }
        void SetDispatcher(Action<Action> dispatch);
        void RaisePropertyChanged(string propertyName);
    }
}
