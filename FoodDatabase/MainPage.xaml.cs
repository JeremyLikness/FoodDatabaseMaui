using FoodDatabase.ViewModels;
using FoodDatabase.Mvvm;

namespace FoodDatabase;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel mainPageViewModel, MvvmInitializer initializer)
	{		
		initializer.Init(this, mainPageViewModel);
		InitializeComponent();		
	}	
}

