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

        public override void Init()
        {
            if (!initialized)
            {
                IsReady = false;
                StatusText = "Database check";
                using var ctx = factory.CreateDbContext();
                if (ctx.Database.EnsureCreated())
                {
                    StatusText = "Database created";
                    Deserialize();
                }
                else
                {
                    StatusText = "Database found";
                }
                initialized = true;
            }
            IsReady = true;
            StatusText = "Navigating to main page";
            Shell.Current.GoToAsync("//Search");
        }        

        private void Deserialize()
        {
            StatusText = "Deserializing data";
            var manifestStreamPath = $"{typeof(FoodContext).Namespace}.foodData.json";
            using var stream = typeof(FoodContext).Assembly.GetManifestResourceStream(manifestStreamPath);
            var doc = JsonDocument.Parse(stream);
            StatusText = "Parsing";
            var data = loader.Parse(doc);
            StatusText = "Saving to database";
            Save(data);
        }

        private void Save(List<FoundationFood> data)
        {
            using var ctx = factory.CreateDbContext();
            ctx.FoundationFoods.AddRange(data);
            ctx.SaveChanges();
            StatusText = "Database seeded";
        }
    }
}
