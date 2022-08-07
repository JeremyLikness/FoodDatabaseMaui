using FoodDatabase.Mvvm;
using FoodDatabase.ViewModels;

namespace FoodDatabase;

public partial class FoodPage : ContentPage
{
    public FoodPage(FoundationFoodViewModel foodsViewModel, MvvmInitializer initializer)
    {
        initializer.Init(this, foodsViewModel);
        InitializeComponent();
    }

}