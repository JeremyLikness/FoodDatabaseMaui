using Microsoft.EntityFrameworkCore;

namespace FoodDatabase.Mvvm
{
    public abstract class BaseDataViewModel<TContext> : BaseViewModel 
        where TContext : DbContext        
    {
        protected IDbContextFactory<TContext> factory;

        public BaseDataViewModel(IDbContextFactory<TContext> dbContextFactory) =>
            factory = dbContextFactory;
    }
}
