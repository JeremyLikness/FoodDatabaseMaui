using FoodDatabase.Data;
using FoodDatabase.Mvvm;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FoodDatabase.ViewModels
{
    public class PowerFoodsViewModel : BaseDataViewModel<FoodContext>
    {

        private readonly List<Nutrient> nutrients = new ();
        private string nutrientSelection = string.Empty;

        public PowerFoodsViewModel(IDbContextFactory<FoodContext> factory) : base(factory)
        {

        }

        public ICommand SearchNavigation { get; private set; } = new NavigationCommand("//Search");

        public string Title => $"Power foods for nutrient {SelectedNutrient?.Name}";

        public List<Nutrient> Nutrients
        {
            get
            {
                if (string.IsNullOrWhiteSpace(nutrientSelection) || nutrientSelection.Length < 3)
                {
                    return new List<Nutrient> { SelectedNutrient };
                }
                var search = nutrientSelection.Trim();
                return nutrients.Where(
                    n => n.Name.StartsWith(search, StringComparison.InvariantCultureIgnoreCase)
                    || n.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
        }

        public ObservableCollection<PowerFoodItem> PowerFoods { get; private set; } = new ();

        private Nutrient nutrient = null;

        public string NutrientSearch
        {
            get => nutrientSelection;
            set
            {
                if (value != nutrientSelection)
                {
                    nutrientSelection = value;
                    RaisePropertyChanged(nameof(NutrientSearch));
                    RaisePropertyChanged(nameof(Nutrients));
                    RaisePropertyChanged(nameof(IsSelecting));
                    Dispatch(async () => await RefreshFoodsAsync());
                }
            }
        }

        public bool IsSelecting => !string.IsNullOrWhiteSpace(nutrientSelection) && nutrientSelection.Trim().Length >= 3;
        
        public Nutrient SelectedNutrient
        {
            get => nutrient;
            set
            {
                if (ReferenceEquals(value, nutrient))
                {
                    return;
                }
                nutrient = value;
                nutrientSelection = string.Empty;
                RaisePropertyChanged(nameof(Title));
                RaisePropertyChanged(nameof(SelectedNutrient));
                RaisePropertyChanged(nameof(Nutrients));
                RaisePropertyChanged(nameof(IsSelecting));
                Dispatch(async () => await RefreshFoodsAsync());
            }
        }

        public override async Task InitAsync()
        {
            using var ctx = await factory.CreateDbContextAsync();
            SetBusy();
            nutrients.AddRange(await ctx.Nutrients.OrderBy(n => n.Name).ToListAsync());
            SelectedNutrient = nutrients.First();
            ResetBusy();
            await RefreshFoodsAsync();
        }

        public async Task RefreshFoodsAsync()
        {
            if (SelectedNutrient == null)
            {
                return;
            }

            SetBusy();
            
            PowerFoods.Clear();
            
            using var ctx = await factory.CreateDbContextAsync();
            
            var powerFoods = ctx.FoundationFoods
                .Include(ff => ff.FoodNutrients)
                .ThenInclude(fn => fn.Nutrient)
            .SelectMany(ff => ff.FoodNutrients,
            (ff, fn) => new
            {
                ff,
                fn
            })
            .Where(r => r.fn.Nutrient.Id == SelectedNutrient.Id)
            .OrderByDescending(r => r.fn.Amount)
            .Take(10)
            .Select(r => new PowerFoodItem
            {
                Id = r.ff.Id,
                Description = r.ff.Description,
                Amount = r.fn.Amount,
                Unit = r.fn.Nutrient.UnitName
            });
            
            PowerFoods.AddRange(await powerFoods.ToListAsync());            
            RaisePropertyChanged(nameof(PowerFoods));
            ResetBusy();
        }
    }
}
