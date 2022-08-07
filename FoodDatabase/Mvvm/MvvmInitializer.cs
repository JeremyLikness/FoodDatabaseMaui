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

        private void Host_Loaded(object sender, EventArgs e)
        {
            if (sender is VisualElement element)
            {
                element.Loaded -= Host_Loaded;
                ((IViewModel)element.BindingContext).Init();
            }
        }
    }
}
