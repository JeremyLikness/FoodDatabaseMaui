using comparison;

// See https://aka.ms/new-console-template for more information
if (args.Length != 2)
{
    Console.WriteLine("Must pass category and food.");
    return;
}

Console.WriteLine("SQLite...");

var example = new SqliteExample();
var category = example.Find(args[0], args[1]);
Show(category);

Console.WriteLine("EF Core...");

var efExample = new EFCoreExample();
category = efExample.Find(args[0], args[1]);
Show(category);

static void Show(FoodCategory category)
{
    Console.WriteLine($"{category.Id} ({category.Code}): {category.Description}");
    foreach (var food in category.FoundationFoods)
    {
        Console.WriteLine($"\t{food.Id}: {food.Description}");
    }
}