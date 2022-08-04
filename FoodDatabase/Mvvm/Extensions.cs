using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace FoodDatabase.Mvvm
{
    public static class Extensions
    {
        public static IServiceCollection WithViewModel<TViewModel>(this IServiceCollection services)
            where TViewModel : BaseViewModel
        {
            services.AddSingleton<TViewModel>();
            return services;
        }

        public static IServiceCollection AddMvvm(this IServiceCollection services)            
        {
            services.AddSingleton<MvvmInitializer>();
            return services;
        }

        public static void NotifySet<TViewModel, TProperty>(
            this TViewModel model, 
            Expression<Func<TViewModel, TProperty>> property,
            Action<TProperty> set,
            TProperty value,
            Func<TProperty, TProperty, bool> comparer = null)
            where TViewModel : BaseViewModel
        {
            var propertyName = ((MemberExpression)property.Body).Member.Name;
            var propertyValue = property.Compile().Invoke(model);
            comparer ??= (p1, p2) => Comparer<TProperty>.Default.Compare(p1, p2) == 0;
            if (comparer(propertyValue, value))
            {
                return;
            }
            set(value);
            model.RaisePropertyChanged(propertyName);
        }

        public static void AddRange<T>(this ObservableCollection<T> target, IEnumerable<T> src)
        {
            foreach(T item in src)
            {
                target.Add(item);
            }
        }
    }
}
