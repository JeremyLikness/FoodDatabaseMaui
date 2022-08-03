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
                    FoodClass = item.GetProperty("foodClass").GetString(),
                    Description = item.GetProperty("description").GetString(),
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
                var description = category.GetProperty("description").GetString();
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
                var catId = catElem.GetProperty("id").GetInt32();
                var category = categories.FirstOrDefault(c => c.Id == catId);
                if (category == null)
                {
                    category = new FoodCategory()
                    {
                        Id = catId,
                        Code = catElem.GetProperty("code").GetString(),
                        Description = catElem.GetProperty("description").GetString()
                    };
                    categories.Add(category);
                }
                var inputFood = new InputFood()
                {
                    Id = inputElem.GetProperty("id").GetInt32(),
                    FoodClass = nestedInput.GetProperty("foodClass").GetString(),
                    FoodCategory = category,
                    Description = inputElem.GetProperty("foodDescription").GetString(),
                    FdcId = nestedInput.GetProperty("fdcId").GetInt32(),
                    DataType = nestedInput.GetProperty("dataType").GetString()
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
                var measureId = measureElem.GetProperty("id").GetInt32();
                var measure = measureUnits.FirstOrDefault(mu => mu.Id == measureId);
                if (measure == null)
                {
                    measure = new MeasureUnit
                    {
                        Id = measureId,
                        Name = measureElem.GetProperty("name").GetString(),
                        Abbreviation = measureElem.GetProperty("abbreviation").GetString()
                    };
                    measureUnits.Add(measure);
                }
                var portion = new FoodPortion
                {
                    Id = portionElem.GetProperty("id").GetInt32(),
                    MeasureUnit = measure
                };
                GetIf(portionElem, "portionDescription",
                    elem => portion.PortionDescription = elem.GetString());
                GetIf(portionElem, "modifier",
                    elem => portion.Modifier = elem.GetString());
                GetIf(portionElem, "gramWeight",
                                    elem => portion.GramWeight = elem.GetDouble());
                GetIf(portionElem, "sequenceNumber",
                                    elem => portion.SequenceNumber = elem.GetInt32());
                GetIf(portionElem, "minYearAcquired",
                                    elem => portion.MinYearAcquired = elem.GetInt32());
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
                    Type = factor.GetProperty("type").GetString()
                };

                GetIf(factor, "proteinValue", elem => newFactor.ProteinValue = elem.GetDouble());
                GetIf(factor, "fatValue", elem => newFactor.FatValue = elem.GetDouble());
                GetIf(factor, "carbohydrateValue", elem => newFactor.CarbohydrateValue = elem.GetDouble());
                GetIf(factor, "value", elem => newFactor.Value = elem.GetDouble());
                nutrientConversionFactors.Add(newFactor);
            }
        }

        private static bool GetIf(
            JsonElement elem,
            string propertyName,
            Action<JsonElement> assign)
        {
            if (elem.TryGetProperty(propertyName, out JsonElement property))
            {
                assign(property);
                return true;
            }

            return false;
        }

        private static void ParseNutrients(List<Nutrient> nutrients, JsonElement item, List<FoodNutrient> foodNutrients)
        {
            var src = item.GetProperty("foodNutrients");
            foreach (var srcItem in src.EnumerateArray())
            {
                var foodNutrientId = srcItem.GetProperty("id").GetInt32();
                var nutrientElem = srcItem.GetProperty("nutrient");
                if (nutrientElem.ValueKind == JsonValueKind.Null)
                {
                    return;
                }
                var id = nutrientElem.GetProperty("id").GetInt32();
                var nutrient = nutrients.FirstOrDefault(n => n.Id == id);
                if (nutrient == null)
                {
                    nutrient = new Nutrient
                    {
                        Id = id,
                        Number = nutrientElem.GetProperty("number").GetString(),
                        Name = nutrientElem.GetProperty("name").GetString(),
                        Rank = nutrientElem.GetProperty("rank").GetInt32()
                    };

                    GetIf(nutrientElem,
                        "unitName",
                        prop => nutrient.UnitName = prop.GetString());
                    nutrients.Add(nutrient);
                }
                var foodNutrient = new FoodNutrient
                {
                    Id = foodNutrientId,
                    Type = srcItem.GetProperty("type").GetString(),
                    Nutrient = nutrient,
                };

                GetIf(srcItem,
                    "dataPoints",
                    prop => foodNutrient.DataPoints = prop.GetInt32());

                GetIf(srcItem,
                    "median",
                    prop => foodNutrient.Median = prop.GetDouble());

                GetIf(srcItem,
                    "amount",
                    prop => foodNutrient.Amount = prop.GetDouble());

                foodNutrients.Add(foodNutrient);
            }
        }

    }
}
