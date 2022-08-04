using Microsoft.EntityFrameworkCore;

namespace FoodDatabase.Data
{
    public class FoodContext : DbContext
    {
        public FoodContext(DbContextOptions<FoodContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
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
