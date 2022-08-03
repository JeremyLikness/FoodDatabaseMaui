namespace FoodDatabase;

public partial class AppShell : Shell
{
	MainPage mainPage;
	SearchPage searchPage;

	public AppShell(
		MainPage mainPage,
		SearchPage searchPage)
	{
		this.mainPage = mainPage;
		this.searchPage = searchPage;
		InitializeComponent();		
	}
		
	private void Shell_Loaded(object sender, EventArgs e)
	{
		MainContainer.Content = mainPage.Content;
		SearchContainer.Content = searchPage.Content;
	}
}
