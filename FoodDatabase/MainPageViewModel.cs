using FoodDatabase.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Text.Json;

namespace FoodDatabase
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private IDbContextFactory<FoodContext> factory;
        private string statusText = "Initializing";

        public MainPageViewModel(IDbContextFactory<FoodContext> factory) =>
            this.factory = factory;            
        
        public string StatusText 
        {
            get => statusText;
            set
            {
                if (statusText != value)
                {
                    statusText = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(StatusText)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task InitAsync()
        {
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
        }

        private async Task DeserializeAsync()
        {
            StatusText = "Deserializing data";
            var manifestStreamPath = $"{typeof(FoodContext).Namespace}.foodData.json";
            using var stream = typeof(FoodContext).Assembly.GetManifestResourceStream(manifestStreamPath);
            var doc = await JsonDocument.ParseAsync(stream);
            StatusText = "Parsing";
            var data = Parse(doc);
            StatusText = "Saving to database";
            await SaveAsync(data);
        }

        private void TryAssign<TEntity, TProp>(
            JsonElement element,
            TEntity entity,
            Action<TProp> assign,
            string propertyName)
        {
            if (element.TryGetProperty(propertyName, out JsonElement prop))
            {
                TProp value;
                if (typeof(TProp) == typeof(int))
                {
                    value = (TProp)((object)prop.GetInt32());
                }
                else if (typeof(TProp) == typeof(double))
                {
                    value = (TProp)((object)prop.GetDouble());
                }
                else value = (TProp)((object)prop.GetString());
                assign(value);
            }
        }
        private static List<FoundationFood> Parse(JsonDocument doc)
        {
            var data = new List<FoundationFood>();
            var nutrients = new List<Nutrient>();

            var root = doc.RootElement.GetProperty(nameof(FoodContext.FoundationFoods));
            foreach(var item in root.EnumerateArray())
            {
                var food = new FoundationFood
                {
                    FoodClass = item.GetProperty("foodClass").GetString(),
                    Description = item.GetProperty("description").GetString(),
                    FoodNutrients = new List<FoodNutrient>()
                };
                ParseNutrients(nutrients, item, food.FoodNutrients);
                data.Add(food);
            }
            return data;
        }

        private static bool GetIf(
            JsonElement elem, 
            string propertyName,
            Action<JsonElement> assign)
        {
            if (elem.TryGetProperty(propertyName, out JsonElement property))
            {
                assign(property);
                return true;
            }

            return false;
        }

        private static void ParseNutrients(List<Nutrient> nutrients, JsonElement item, List<FoodNutrient> foodNutrients)
        {
            var src = item.GetProperty("foodNutrients");
            foreach (var srcItem in src.EnumerateArray())
            {
                var foodNutrientId = srcItem.GetProperty("id").GetInt32();
                var nutrientElem = srcItem.GetProperty("nutrient");
                if (nutrientElem.ValueKind == JsonValueKind.Null)
                {
                    return;
                }
                var id = nutrientElem.GetProperty("id").GetInt32();
                var nutrient = nutrients.FirstOrDefault(n => n.Id == id);
                if (nutrient == null)
                {
                    nutrient = new Nutrient
                    {
                        Id = id,
                        Number = nutrientElem.GetProperty("number").GetString(),
                        Name = nutrientElem.GetProperty("name").GetString(),
                        Rank = nutrientElem.GetProperty("rank").GetInt32()
                    };

                    GetIf(nutrientElem,
                        "unitName",
                        prop => nutrient.UnitName = prop.GetString());
                    nutrients.Add(nutrient);
                }
                var foodNutrient = new FoodNutrient
                {
                    Id = foodNutrientId,
                    Type = srcItem.GetProperty("type").GetString(),
                    Nutrient = nutrient,
                };
                
                GetIf(srcItem,
                    "dataPoints",
                    prop => foodNutrient.DataPoints = prop.GetInt32());

                GetIf(srcItem,
                    "median",
                    prop => foodNutrient.Median = prop.GetDouble());

                GetIf(srcItem,
                    "amount",
                    prop => foodNutrient.Amount = prop.GetDouble());

                foodNutrients.Add(foodNutrient);
            }
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
