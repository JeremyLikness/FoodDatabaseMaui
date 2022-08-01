namespace FoodDatabase;

public partial class AppShell : Shell
{
	MainPageViewModel vm;
	public AppShell(MainPageViewModel vm)
	{
		this.vm = vm;
		InitializeComponent();		
	}

	private void Shell_Loaded(object sender, EventArgs e) =>
		ShellContainer.BindingContext = vm;	
}
