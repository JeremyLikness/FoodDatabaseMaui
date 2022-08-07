using Microsoft.Data.Sqlite;

namespace comparison;

public class SqliteExample
{
    public FoodCategory? Find(string category, string food)
    {
        using var connection = new SqliteConnection("Data Source=foods.sqlite3");
        connection.Open();

        using var cmd = new SqliteCommand(
            @"SELECT fc.Id as fcId, 
                     fc.Description as fcDescription, 
                     fc.Code as fcCode,
                     ff.Id as ffId,
                     ff.Description as ffDescription
             FROM FoodCategories fc
             INNER JOIN FoundationFoods ff on ff.FoodCategoryId == fc.Id
             WHERE fc.Description LIKE @foodcat AND ff.Description LIKE @food
             ORDER BY ff.Description", connection);        

        var categoryParam = new SqliteParameter
        { 
            ParameterName = "@foodcat",
            Value = $"%{category}%"
        };

        cmd.Parameters.Add(categoryParam);

        var foodParam = new SqliteParameter
        {
            ParameterName = "@food",
            Value = $"%{food}%"
        };

        cmd.Parameters.Add(foodParam);

        var result = new FoodCategory();

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            if (result.Id == 0)
            {
                result.Id = Convert.ToInt32((long)reader["fcId"]);
                result.Description = (string)(reader["fcDescription"]);
                result.Code = (string)(reader["fcCode"]);
            }
            var foundationFood = new FoundationFood()
            {
                Id = Convert.ToInt32((long)reader["ffId"]),
                Description = (string)reader["ffDescription"],
                FoodCategory = result
            };
            result.FoundationFoods.Add(foundationFood);           
        }

        return result;
    }
}