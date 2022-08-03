using FoodDatabase.Data;
using FoodDatabase.Mvvm;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FoodDatabase.ViewModels
{
    public class MainPageViewModel : BaseDataViewModel<FoodContext>
    {
        private readonly DataLoader loader;
        private string statusText = "Initializing";
        private bool ready = false;
        private bool initialized = false;

        public MainPageViewModel(
            IDbContextFactory<FoodContext> factory, DataLoader loader)
            : base(factory) => this.loader = loader;       

        public string StatusText
        {
            get => statusText;
            set => this.NotifySet(vm => vm.StatusText, val => statusText = val, value);
        }

        public bool IsReady
        {
            get => ready;
            set => this.NotifySet(vm => vm.IsReady, val => ready = val, value);
        }

        public async override Task InitAsync()
        {
            if (!initialized)
            {
                IsReady = false;
                StatusText = "Database check";
                using var ctx = await factory.CreateDbContextAsync();
                await ctx.Database.EnsureDeletedAsync();
                if (await ctx.Database.EnsureCreatedAsync())
                {
                    StatusText = "Database created";
                    await DeserializeAsync();
                }
                else
                {
                    StatusText = "Database found";
                }
                initialized = true;
            }
            IsReady = true;
            StatusText = "Navigating to main page";
            await Shell.Current.GoToAsync("//Search");
        }        

        private async Task DeserializeAsync()
        {
            StatusText = "Deserializing data";
            var manifestStreamPath = $"{typeof(FoodContext).Namespace}.foodData.json";
            using var stream = typeof(FoodContext).Assembly.GetManifestResourceStream(manifestStreamPath);
            var doc = await JsonDocument.ParseAsync(stream);
            StatusText = "Parsing";
            var data = loader.Parse(doc);
            StatusText = "Saving to database";
            await SaveAsync(data);
        }

        private async Task SaveAsync(List<FoundationFood> data)
        {
            using var ctx = await factory.CreateDbContextAsync();
            ctx.FoundationFoods.AddRange(data);
            await ctx.SaveChangesAsync();
            StatusText = "Database seeded";
        }
    }
}
