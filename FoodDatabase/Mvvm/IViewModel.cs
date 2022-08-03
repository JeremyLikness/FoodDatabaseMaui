namespace FoodDatabase.Mvvm
{
    public interface IViewModel
    {
        Task InitAsync();
        Action<Action> Dispatch { get; }
        void SetDispatcher(Action<Action> dispatch);
        void RaisePropertyChanged(string propertyName);
    }
}
