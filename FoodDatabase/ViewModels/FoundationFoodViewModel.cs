using FoodDatabase.Data;
using FoodDatabase.Mvvm;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace FoodDatabase.ViewModels
{
    public class FoundationFoodViewModel : BaseDataViewModel<FoodContext>
    {      
        public int FoodId { get; set; }

        public ICommand BackCommand { get; private set; }

        private FoundationFood food = null;

        public FoundationFood Food
        {
            get => food;
            set
            {
                food = value;                
                RaisePropertyChanged(nameof(Food));
            }
        }

        public FoundationFoodViewModel(IDbContextFactory<FoodContext> factory) : base(factory)
        {
        }

        public FoodNutrient NutrientByName(string name, bool contains = false) =>
            contains
            ? Food?.FoodNutrients
                .Where(fn => fn.Nutrient.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(fn => fn.Nutrient.Rank).FirstOrDefault() 
            : Food?.FoodNutrients
                .Where(fn => fn.Nutrient.Name == name)
                .OrderBy(fn => fn.Nutrient.Rank).FirstOrDefault();

        public FoodNutrient Calories => NutrientByName("Energy", true);
        public FoodNutrient Fat => NutrientByName("Total fat", true);
        public FoodNutrient SaturatedFat => NutrientByName("Fatty acids, total saturated");
        public FoodNutrient TransFat => NutrientByName("Fatty acids, total trans");
        public FoodNutrient Cholesterol => NutrientByName("Cholesterol",  true);
        public FoodNutrient Sodium => NutrientByName("Sodium", true);
        public FoodNutrient Carbohydrate => NutrientByName("Carbohydrate", true);
        public FoodNutrient Fiber => NutrientByName("Fiber, total", true);
        public FoodNutrient Sugars => NutrientByName("Sugars, Total", true);
        public FoodNutrient Protein => NutrientByName("Protein", true);
        public FoodNutrient VitaminD => NutrientByName("Vitamin D", true);
        public FoodNutrient Iron => NutrientByName("Iron", true);
        public FoodNutrient VitaminA => NutrientByName("Vitamin A", true);
        public FoodNutrient Riboflavin => NutrientByName("Riboflavin", true);
        public FoodNutrient VitaminB6 => NutrientByName("Vitamin B6", true);
        public FoodNutrient Calcium => NutrientByName("Calcium", true);
        public FoodNutrient Potassium => NutrientByName("Potassium", true);
        public FoodNutrient Thiamin => NutrientByName("Thiamin", true);
        public FoodNutrient Niacin => NutrientByName("Niacin", true);
        public FoodNutrient Zinc => NutrientByName("Zinc", true);

        public override void Init()
        {
            if (Food == null || Food.Id != FoodId)
            {
                using var ctx = factory.CreateDbContext();
                Food = ctx.FoundationFoods
                    .Include(ff => ff.FoodNutrients)
                    .ThenInclude(fn => fn.Nutrient)
                    .Include(ff => ff.FoodPortions)
                    .ThenInclude(fp => fp.MeasureUnit)
                    .SingleOrDefault(ff => ff.Id == FoodId);
            }
            RaisePropertyChanged(string.Empty);
        }

        public async Task ShowFoodAsync(int foodId, string returnRoute)
        {
            FoodId = foodId;
            BackCommand = new NavigationCommand(returnRoute);
            await Shell.Current.GoToAsync("//Food");
            Init();
        }
    }
}
