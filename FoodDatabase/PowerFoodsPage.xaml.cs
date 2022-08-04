using FoodDatabase.Mvvm;
using FoodDatabase.ViewModels;

namespace FoodDatabase;

public partial class PowerFoodsPage : ContentPage
{
    public PowerFoodsPage(PowerFoodsViewModel powerFoodsViewModel, MvvmInitializer initializer)
    {
        initializer.Init(this, powerFoodsViewModel);
        InitializeComponent();
    }
}