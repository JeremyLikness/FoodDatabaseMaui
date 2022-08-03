using Microsoft.EntityFrameworkCore;

namespace FoodDatabase.Data
{
    public class FoodContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var pathToLocal = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            optionsBuilder.UseSqlite($"Data Source={pathToLocal}{Path.DirectorySeparatorChar}foods.sqlite3");
            optionsBuilder.AddInterceptors(new[] { new SearchDataInterceptor() });
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<FoundationFood> FoundationFoods { get; set; }
        public DbSet<FoodCategory> FoodCategories { get; set; }
        public DbSet<MeasureUnit> MeasureUnits { get; set; }
        public DbSet<Nutrient> Nutrients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var ff = modelBuilder.Entity<FoundationFood>();

            ff.Property<string>("SearchData");

            ff.OwnsMany(ff => ff.FoodNutrients);

            ff.OwnsMany(ff => ff.NutrientConversionFactors, a =>
            {
                a.Property<int>("Id");
                a.HasKey("Id");
            }); 

            ff.OwnsMany(ff => ff.FoodPortions)
                .HasOne(fp => fp.MeasureUnit)
                .WithMany();
            
            modelBuilder.Entity<InputFood>()
                .HasOne(inf => inf.FoodCategory);
                       
            base.OnModelCreating(modelBuilder);
        }
    }
}
