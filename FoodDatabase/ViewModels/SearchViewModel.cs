using FoodDatabase.Data;
using FoodDatabase.Mvvm;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FoodDatabase.ViewModels
{
    public class SearchViewModel : BaseDataViewModel<FoodContext>
    {
        private const int DebounceMs = 300;
        private string searchText;
        private readonly List<FoundationFood> foundationFoods = new();
        private FoodCategory category = null;
        private readonly List<FoodCategory> foodCategories = new();
        private Timer debounceTimer;
        private bool showCategory;
        
        private string databaseStats = "Computing statistics...";

        public List<FoodCategory> FoodCategories => foodCategories.ToList();

        public string DatabaseStats
        {
            get => databaseStats;
            set => this.NotifySet(svm => svm.DatabaseStats, val => databaseStats = val, value);            
        }

        public string CategoryText => category?.Description;

        public ICommand PowerFoodsNavigation { get; set; } = new NavigationCommand("//PowerFoods");

        public ICommand ShowCategoryCommand { get; private set; }

        public ICommand FoodCommand { get; private set; }

        public ObservableCollection<FoundationFood> FoundationFoods { get; private set; }
            = new ObservableCollection<FoundationFood>();
       
        public SearchViewModel(IDbContextFactory<FoodContext> factory, FoundationFoodViewModel vm) : base(factory) 
        {
            ShowCategoryCommand = new Command(
                () =>
                {
                    showCategory = true;
                    RaisePropertyChanged(nameof(ShowCategory));
                    ((Command)ShowCategoryCommand).ChangeCanExecute();
                },
                () => ShowCategory == false);
            FoodCommand = new Command(
                (object id) => _ = vm.ShowFoodAsync((int)id, "//Search"),
                _ => true);
        }

        public bool ShowCategory => showCategory;        
        
        public async override Task InitAsync()
        {
          SetBusy();
            using var context = await factory.CreateDbContextAsync();
        var count = await context.FoundationFoods.CountAsync();
        var nutrientsCount = await context.FoundationFoods
            .SelectMany(f => f.FoodNutrients)
            .Select(fn => fn.Nutrient)
            .Where(n => n != null)
            .Select(n => n.Id)
            .Distinct()
            .CountAsync();
        foodCategories.AddRange(await context.FoodCategories.OrderBy(fc => fc.Description).ToListAsync());
        category = foodCategories.First();
        RaisePropertyChanged(nameof(FoodCategories));
        RaisePropertyChanged(nameof(Category));
        ResetBusy();
        DatabaseStats = $"{count} food items and {nutrientsCount} nutrients found.";
    }

        public FoodCategory Category
        {
            get => category;
            set => this.NotifySet(svm => svm.Category, val =>
            {
                category = val;
                if (showCategory)
                {
                    showCategory = false;
                    RaisePropertyChanged(nameof(ShowCategory));
                    ((Command)ShowCategoryCommand).ChangeCanExecute();
                }
            }, value, (p1, p2) => p1 != null && p2 != null && p1.Id == p2.Id);            
        }

        public string SearchText
        {
            get => searchText;
            set
            {
                if (searchText != value)
                {
                    searchText = value.Trim();
                    RaisePropertyChanged(nameof(SearchPage));
                    if (!string.IsNullOrWhiteSpace(searchText) &&
                        searchText.Length >= 3)
                    {
                        var _ = DebounceAsync();
                    }
                    else
                    {
                        FoundationFoods.Clear();
                        RaisePropertyChanged(nameof(FoundationFoods));
                        RaisePropertyChanged(nameof(HasFoods));
                        RaisePropertyChanged(nameof(NoFoods));
                    }
                }
            }
        }

        public bool HasFoods => FoundationFoods.Count > 0;

        public bool NoFoods => FoundationFoods.Count == 0;

        private async Task DebounceAsync()
        {
            if (debounceTimer != null)
            {
                await debounceTimer.DisposeAsync();
                debounceTimer = null;
            }

            if (string.IsNullOrWhiteSpace(searchText) ||
                searchText.Length < 3)
            {
                return;
            }

            debounceTimer = new Timer(
                TimerCallbackHandler,
                this,
                DebounceMs,
                Timeout.Infinite);
        }

        private void TimerCallbackHandler(object sender)
        {
            debounceTimer?.Dispose();
            debounceTimer = null;
            if (!string.IsNullOrWhiteSpace(searchText) &&
                searchText.Length >= 3)
            {
                base.Dispatch(async () => await SearchAsync());
            }
            else
            {
                RaisePropertyChanged(nameof(HasFoods));
                RaisePropertyChanged(nameof(NoFoods));
            }
        }

        private async Task SearchAsync()
        {
            var search = searchText.Trim().ToLowerInvariant();
            SetBusy();
            FoundationFoods.Clear();
            RaisePropertyChanged(nameof(FoundationFoods));
            var context = await factory.CreateDbContextAsync();
            var results = await context.FoundationFoods
                .Where(ff =>
                ff.FoodCategory.Id == category.Id &&
                EF.Property<string>(ff, "SearchData")
                .Contains(search))
                .OrderBy(ff => ff.Description)
                .ToListAsync();
            foreach (var result in results)
            {
                FoundationFoods.Add(result);
            }
            ResetBusy();
            RaisePropertyChanged(nameof(FoundationFoods));
            RaisePropertyChanged(nameof(HasFoods));
            RaisePropertyChanged(nameof(NoFoods));            
        }
    }
}
