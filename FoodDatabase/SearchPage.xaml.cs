using FoodDatabase.Mvvm;
using FoodDatabase.ViewModels;

namespace FoodDatabase;

public partial class SearchPage : ContentPage
{
	public SearchPage(SearchViewModel searchViewModel, MvvmInitializer initializer)
	{
		initializer.Init(this, searchViewModel);
		InitializeComponent();
		
	}
	
	private void Button_Clicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//PowerFoods");
	}
}