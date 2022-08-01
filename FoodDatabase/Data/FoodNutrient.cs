using Microsoft.EntityFrameworkCore;

namespace FoodDatabase.Data
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class FoodCategory
    {
        public string Description { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
    }

    public class FoodNutrient
    {
        public string Type { get; set; }
        public int Id { get; set; }
        public Nutrient Nutrient { get; set; }
        public int DataPoints { get; set; }
        public double Median { get; set; }
        public double Amount { get; set; }
        public double? Max { get; set; }
        public double? Min { get; set; }
    }
    
    public class FoodPortion
    {
        public int Id { get; set; }
        public MeasureUnit MeasureUnit { get; set; }
        public string Modifier { get; set; }
        public double GramWeight { get; set; }
        public int SequenceNumber { get; set; }
        public int MinYearAcquired { get; set; }
        public string PortionDescription { get; set; }
    }

    public class FoundationFood
    {
        public int Id { get; set; }
        public string FoodClass { get; set; }
        public string Description { get; set; }
        public List<FoodNutrient> FoodNutrients { get; set; }

        public List<NutrientConversionFactor> NutrientConversionFactors { get; set; }
        public bool IsHistoricalReference { get; set; }
        public int NdbNumber { get; set; }
        public int FdcId { get; set; }
        public string DataType { get; set; }
        public FoodCategory FoodCategory { get; set; }
        public List<FoodPortion> FoodPortions { get; set; }
        public List<InputFood> InputFoods { get; set; }
        public string PublicationDate { get; set; }
        public string ScientificName { get; set; }
    }

    public class InputFood
    {
        public int Id { get; set; }
        public string FoodClass { get; set; }
        public string Description { get; set; }
        public int FdcId { get; set; }
        public string DataType { get; set; }
        public FoodCategory FoodCategory { get; set; }
        public string PublicationDate { get; set; }
    }

    public class MeasureUnit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }

    public class Nutrient
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public string UnitName { get; set; }
    }

    public class NutrientConversionFactor
    {
        public string Type { get; set; }
        public double ProteinValue { get; set; }
        public double FatValue { get; set; }
        public double CarbohydrateValue { get; set; }
        public double? Value { get; set; }
    }

    public class Root
    {
        public List<FoundationFood> FoundationFoods { get; set; }
    }

}
