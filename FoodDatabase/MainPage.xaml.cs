namespace FoodDatabase;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void ContentPage_Loaded(object sender, EventArgs e)
	{
		if (BindingContext is MainPageViewModel vm)
		{
			await vm.InitAsync();
		}
	}
}

