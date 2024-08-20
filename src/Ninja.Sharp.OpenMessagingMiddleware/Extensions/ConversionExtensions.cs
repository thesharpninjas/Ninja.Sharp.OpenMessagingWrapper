using System.Text.Json;
using System.Text.Json.Serialization;

namespace System
{
    public static class ConversionExtensions
    {

        private static readonly JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private static readonly JsonSerializerOptions optionsNotIgnoringNull = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        public static string Serialize<T>(this T item, bool ignoreNull = true)
        {
            JsonSerializerOptions myOptions = options;
            if (!ignoreNull)
            {
                myOptions = optionsNotIgnoringNull;
            }
            return JsonSerializer.Serialize(item, myOptions);
        }

        public static T Deserialize<T>(this string item, bool ignoreNull = true)
        {
            JsonSerializerOptions myOptions = options;
            if (!ignoreNull)
            {
                myOptions = optionsNotIgnoringNull;
            }

            string? requestedTypeName = typeof(T).FullName;
            if (string.IsNullOrWhiteSpace(item))
            {
                throw new ArgumentException("Impossibile convertire in " + requestedTypeName);
            }
            try
            {
                T? myItem = JsonSerializer.Deserialize<T>(item, myOptions);

                return myItem is null ? throw new ArgumentException("Impossibile convertire in " + requestedTypeName) : myItem;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Impossibile convertire in " + requestedTypeName, ex);
            }

        }
    }
}
