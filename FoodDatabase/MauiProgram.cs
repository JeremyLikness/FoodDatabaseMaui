using FoodDatabase.Data;

namespace FoodDatabase;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.
			Services.AddDbContextFactory<FoodContext>()
			.AddSingleton<AppShell>()
			.AddSingleton<MainPageViewModel>();

		return builder.Build();
	}
}
