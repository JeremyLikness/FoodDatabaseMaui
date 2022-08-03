namespace FoodDatabase.Mvvm
{
    public class MvvmInitializer
    {
        public void Init(VisualElement host, IViewModel viewModel)
        {
            host.BindingContext = viewModel;
            viewModel.SetDispatcher(action => host.Dispatcher.DispatchAsync(action));
            host.Loaded += Host_Loaded;
        }

        private async void Host_Loaded(object sender, EventArgs e)
        {
            if (sender is VisualElement element)
            {
                element.Loaded -= Host_Loaded;
                await ((IViewModel)element.BindingContext).InitAsync();
            }
        }
    }
}
