using System.Text.Json;
using System.Text.Json.Serialization;

namespace System
{
    internal static class ConversionExtensions
    {

        private static readonly JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static string Serialize<T>(this T item)
        {
            JsonSerializerOptions myOptions = options;
            return JsonSerializer.Serialize(item, myOptions);
        }

        public static T Deserialize<T>(this string item, bool ignoreNull = true)
        {
            JsonSerializerOptions myOptions = options;

            string? requestedTypeName = typeof(T).FullName;
            if (string.IsNullOrWhiteSpace(item))
            {
                throw new ArgumentException("Cannot convert in " + requestedTypeName);
            }
            try
            {
                T? myItem = JsonSerializer.Deserialize<T>(item, myOptions);

                return myItem is null ? throw new ArgumentException("Cannot convert in " + requestedTypeName) : myItem;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Cannot convert in " + requestedTypeName, ex);
            }
        }
    }
}