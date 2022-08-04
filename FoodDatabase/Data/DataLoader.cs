using System.Text.Json;

namespace FoodDatabase.Data
{
    public class DataLoader
    {
        public List<FoundationFood> Parse(JsonDocument doc)
        {
            var data = new List<FoundationFood>();
            var nutrients = new List<Nutrient>();
            var measureUnits = new List<MeasureUnit>();
            var categories = new List<FoodCategory>();

            var root = doc.RootElement.GetProperty(nameof(FoodContext.FoundationFoods));
            foreach (var item in root.EnumerateArray())
            {
                var food = new FoundationFood
                {
                    FoodClass = item.GetPropertyAs<string>("foodClass"),
                    Description = item.GetPropertyAs<string>("description"),
                    FoodNutrients = new List<FoodNutrient>(),
                    NutrientConversionFactors = new List<NutrientConversionFactor>(),
                    FoodPortions = new List<FoodPortion>(),
                    InputFoods = new List<InputFood>()
                };
                ParseNutrients(nutrients, item, food.FoodNutrients);
                ParseConversions(item, food.NutrientConversionFactors);
                ParsePortions(measureUnits, item, food.FoodPortions);
                ParseInputFoods(categories, item, food.InputFoods);
                var category = item.GetProperty("foodCategory");
                var description = category.GetPropertyAs<string>("description");
                food.FoodCategory = categories.FirstOrDefault(c => c.Description == description);
                data.Add(food);
            }
            return data;
        }

        private static void ParseInputFoods(List<FoodCategory> categories, JsonElement item, List<InputFood> inputFoods)
        {
            var inputs = item.GetProperty("inputFoods");
            foreach (var inputElem in inputs.EnumerateArray())
            {
                var nestedInput = inputElem.GetProperty("inputFood");
                var catElem = nestedInput.GetProperty("foodCategory");
                var catId = catElem.GetPropertyAs<int>("id");
                var category = categories.FirstOrDefault(c => c.Id == catId);
                if (category == null)
                {
                    category = new FoodCategory()
                    {
                        Id = catId,
                        Code = catElem.GetPropertyAs<string>("code"),
                        Description = catElem.GetPropertyAs<string>("description")
                    };
                    categories.Add(category);
                }
                var inputFood = new InputFood()
                {
                    Id = inputElem.GetPropertyAs<int>("id"),
                    FoodClass = nestedInput.GetPropertyAs<string>("foodClass"),
                    FoodCategory = category,
                    Description = inputElem.GetPropertyAs<string>("foodDescription"),
                    FdcId = nestedInput.GetPropertyAs<int>("fdcId"),
                    DataType = nestedInput.GetPropertyAs<string>("dataType")
                };
                inputFoods.Add(inputFood);
            }            
        }

        private static void ParsePortions(List<MeasureUnit> measureUnits, JsonElement item, List<FoodPortion> foodPortions)
        {
            var portionElements = item.GetProperty("foodPortions");
            foreach (var portionElem in portionElements.EnumerateArray())
            {
                var measureElem = portionElem.GetProperty("measureUnit");
                var measureId = measureElem.GetPropertyAs<int>("id");
                var measure = measureUnits.FirstOrDefault(mu => mu.Id == measureId);
                if (measure == null)
                {
                    measure = new MeasureUnit
                    {
                        Id = measureId,
                        Name = measureElem.GetPropertyAs<string>("name"),
                        Abbreviation = measureElem.GetPropertyAs<string>("abbreviation")
                    };
                    measureUnits.Add(measure);
                }
                var portion = new FoodPortion
                {
                    Id = portionElem.GetPropertyAs<int>("id"),
                    MeasureUnit = measure,
                    PortionDescription = portionElem.GetPropertyWithDefault("portionDescription", string.Empty),
                    Modifier = portionElem.GetPropertyWithDefault("modifier", string.Empty),
                    GramWeight = portionElem.GetPropertyWithDefault("gramWeight", (double)0),
                    SequenceNumber = portionElem.GetPropertyWithDefault("sequenceNumber", int.MaxValue),
                    MinYearAcquired = portionElem.GetPropertyWithDefault("minYearAcquired", DateTime.Now.Year)
                };

                foodPortions.Add(portion);
            }
        }

        private static void ParseConversions(JsonElement item, List<NutrientConversionFactor> nutrientConversionFactors)
        {
            var factors = item.GetProperty("nutrientConversionFactors");
            foreach (var factor in factors.EnumerateArray())
            {
                var newFactor = new NutrientConversionFactor
                {
                    Type = factor.GetProperty("type").GetString(),
                    ProteinValue = factor.GetPropertyWithDefault("proteinValue", (double)0),
                    FatValue = factor.GetPropertyWithDefault("fatValue", (double)0),
                    CarbohydrateValue = factor.GetPropertyWithDefault("carbohydrateValue", (double)0),
                    Value = factor.GetPropertyWithDefault("value", (double)0),
                };

                nutrientConversionFactors.Add(newFactor);
            }
        }

        private static void ParseNutrients(List<Nutrient> nutrients, JsonElement item, List<FoodNutrient> foodNutrients)
        {
            var src = item.GetProperty("foodNutrients");
            foreach (var srcItem in src.EnumerateArray())
            {
                var foodNutrientId = srcItem.GetPropertyAs<int>("id");
                var nutrientElem = srcItem.GetProperty("nutrient");
                if (nutrientElem.ValueKind == JsonValueKind.Null)
                {
                    return;
                }
                var id = nutrientElem.GetPropertyAs<int>("id");
                var nutrient = nutrients.FirstOrDefault(n => n.Id == id);
                if (nutrient == null)
                {
                    nutrient = new Nutrient
                    {
                        Id = id,
                        Number = nutrientElem.GetPropertyAs<string>("number"),
                        Name = nutrientElem.GetPropertyAs<string>("name"),
                        Rank = nutrientElem.GetPropertyAs<int>("rank"),
                        UnitName = nutrientElem.GetPropertyWithDefault("unitName", "??")
                    };

                    nutrients.Add(nutrient);
                }
                var foodNutrient = new FoodNutrient
                {
                    Id = foodNutrientId,
                    Type = srcItem.GetPropertyAs<string>("type"),
                    Nutrient = nutrient,
                    DataPoints = srcItem.GetPropertyWithDefault("dataPoints", 0),
                    Median = srcItem.GetPropertyWithDefault("median", (double)0),
                    Amount = srcItem.GetPropertyWithDefault("amount", (double)0)
                };                
                foodNutrients.Add(foodNutrient);
            }
        }

    }
}
