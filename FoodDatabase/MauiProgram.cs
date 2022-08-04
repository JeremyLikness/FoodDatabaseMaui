using FoodDatabase.Data;
using FoodDatabase.ViewModels;
using FoodDatabase.Mvvm;
using Microsoft.EntityFrameworkCore;

namespace FoodDatabase;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

        var pathToLocal = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);



        builder

            .UseMauiApp<App>()

			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})

			.Services.AddDbContextFactory<FoodContext>(opts =>
			{
				opts.UseSqlite($"Data Source={pathToLocal}{Path.DirectorySeparatorChar}foods.sqlite3");
				opts.LogTo(Console.WriteLine);
			})            

			.AddMvvm()

			.AddSingleton<AppShell>()

			.AddTransient<MainPage>()
			.WithViewModel<MainPageViewModel>()

			.AddTransient<SearchPage>()
			.WithViewModel<SearchViewModel>()

			.AddTransient<PowerFoodsPage>()
			.WithViewModel<PowerFoodsViewModel>()

			.AddSingleton<DataLoader>();					

		return builder.Build();
	}
}
