using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IRT.Modules.DataTransfer.Generic.Helpers.Extensions
{
    public static class JsonExtensions
    {
        public static bool CanFormatToJson(this string jsonString)
        {
            try
            {
                JObject.Parse(jsonString);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        public static string JsonPrettify(this string json)
        {
            using var jDoc = JsonDocument.Parse(json);
            return System.Text.Json.JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
