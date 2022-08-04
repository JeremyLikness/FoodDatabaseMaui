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
}