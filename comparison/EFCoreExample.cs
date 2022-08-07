using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace comparison;

public class FoodContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=foods.sqlite3")
        .LogTo(Console.WriteLine, LogLevel.Information);
    public DbSet<FoodCategory> FoodCategories { get; set; } = null!;
    public DbSet<FoundationFood> FoundationFoods { get; set; } = null!;
}

public class EFCoreExample
{
    public FoodCategory? Find(string category, string food)
    {
        using var ctx = new FoodContext();
        return ctx.FoodCategories
            .Include(fc => fc.FoundationFoods
                .Where(ff => ff.Description!.Contains(food))
                .OrderBy(ff => ff.Description))
            .FirstOrDefault(fc => fc.Description!.Contains(category));
    }
}