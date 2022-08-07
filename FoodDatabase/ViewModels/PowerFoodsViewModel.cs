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

        public PowerFoodsViewModel(IDbContextFactory<FoodContext> factory, FoundationFoodViewModel vm) : base(factory)
        {
            FoodCommand = new Command(
                (object id) => _ = vm.ShowFoodAsync((int)id, "//PowerFoods"),
                _ => true);
        }

        public ICommand FoodCommand { get; private set; }

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
                    RaisePropertyChanged(nameof(IsNotSelecting));
                    Dispatch(RefreshFoods);
                }
            }
        }

        public bool IsSelecting => !string.IsNullOrWhiteSpace(nutrientSelection) && nutrientSelection.Trim().Length >= 3;
        public bool IsNotSelecting => !IsSelecting;
        
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
                RaisePropertyChanged(nameof(IsNotSelecting));
                Dispatch(RefreshFoods);
            }
        }

        public override void Init()
        {
            using var ctx = factory.CreateDbContext();
            SetBusy();
            nutrients.AddRange(ctx.Nutrients.OrderBy(n => n.Name).ToList());
            SelectedNutrient = nutrients.First();
            ResetBusy();
            RefreshFoods();
        }

        public void RefreshFoods()
        {
            if (SelectedNutrient == null)
            {
                return;
            }

            SetBusy();
            
            PowerFoods.Clear();
            
            using var ctx = factory.CreateDbContext();
            
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
            
            PowerFoods.AddRange(powerFoods.ToList());            
            RaisePropertyChanged(nameof(PowerFoods));
            ResetBusy();
        }
    }
}
