using System.Text.Json;

namespace FoodDatabase.Data
{
    public static class Extensions
    {
        public static T GetPropertyAs<T>(this JsonElement element, string propertyName)
        {
            var prop = element.GetProperty(propertyName);
            if (typeof(T) == typeof(string))
            {
                return (T)(object)prop.GetString();
            }

            if (typeof(T) == typeof(int))
            {
                return (T)(object)prop.GetInt32();
            }

            if (typeof(T) == typeof(double))
            {
                return (T)(object)prop.GetDouble();
            }

            throw new InvalidOperationException($"I don't know how to convert to {typeof(T).Name}.");
        }

        public static T GetPropertyWithDefault<T>(this JsonElement element, string propertyName, T defaultValue)
        {
            if (element.TryGetProperty(propertyName, out JsonElement prop))
            {
                if (prop.ValueKind == JsonValueKind.Null || prop.ValueKind == JsonValueKind.Undefined)
                {
                    return defaultValue;
                }
                return GetPropertyAs<T>(element, propertyName);
            }
            return defaultValue;
        }
    }
}
