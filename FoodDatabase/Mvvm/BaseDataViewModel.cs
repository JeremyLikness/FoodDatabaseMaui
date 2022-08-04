using Microsoft.EntityFrameworkCore;

namespace FoodDatabase.Mvvm
{
    public abstract class BaseDataViewModel<TContext> : BaseViewModel 
        where TContext : DbContext        
    {
        private int busyCount;

        protected IDbContextFactory<TContext> factory;

        public BaseDataViewModel(IDbContextFactory<TContext> dbContextFactory) =>
            factory = dbContextFactory;

        public bool IsBusy { get => busyCount > 0; }

        protected void SetBusy()
        {
            busyCount++;
            if (busyCount == 1)
            {
                RaisePropertyChanged(nameof(IsBusy));
            }
        }

        protected void ResetBusy()
        {
            busyCount--;
            if (busyCount == 0)
            {
                RaisePropertyChanged(nameof(IsBusy));
            }
        }

    }
}
