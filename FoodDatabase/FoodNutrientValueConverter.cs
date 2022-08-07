using FoodDatabase.Data;
using System.Globalization;

namespace FoodDatabase;

public class FoodNutrientValueConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is FoodNutrient fn)
		{
			return $"{fn.Amount} {fn.Nutrient?.UnitName}";				
		}

		return string.Empty;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => new FoodNutrient();
}