using System.Windows.Input;

namespace FoodDatabase.Mvvm
{
    public class NavigationCommand : ICommand
    {
        private readonly string route;

        public NavigationCommand(string route) => this.route = route;

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());

        public bool CanExecute(object parameter)
        {
            if (parameter is bool condition)
            {
                return condition;
            }

            return true;
        }
        
        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                Shell.Current.GoToAsync(route);
            }
        }
    }
}
