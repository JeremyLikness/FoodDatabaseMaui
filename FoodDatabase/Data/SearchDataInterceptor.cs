using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FoodDatabase.Data
{
    public class SearchDataInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ParseSearchData(eventData);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            ParseSearchData(eventData);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void ParseSearchData(DbContextEventData eventData)
        {
            if (eventData != null && eventData.Context != null)
            {
                var interceptorActions = new List<Action>();
                var changes = eventData.Context.ChangeTracker.Entries();
                foreach (var change in changes)
                {
                    if (change.Entity is FoundationFood ff)
                    {
                        var text = $"{ff.Description} {ff.ScientificName}"
                            .Trim()
                            .ToLowerInvariant();
                        interceptorActions.Add(() =>
                        {
                            eventData.Context.Entry(ff).
                            Property<string>("SearchData")
                            .CurrentValue = text;
                        });
                    }
                }

                foreach(var interceptorAction in interceptorActions)
                {
                    interceptorAction();
                }
            }
        }
    }
}
