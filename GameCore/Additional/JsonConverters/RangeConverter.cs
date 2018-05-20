using System;
using System.Globalization;
using GameCore.EMath;
using Newtonsoft.Json;

namespace GameCore.Additional.JsonConverters
{
    public class RangeConverter : JsonConverter<Range>
    {
        public override void WriteJson(JsonWriter writer, Range value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override Range ReadJson(JsonReader reader, Type objectType, Range existingValue,bool hasExistingValue, JsonSerializer serializer)
        {
            var s = ((string) reader.Value).Split('-');

            if (s.Length == 2 &&
                float.TryParse(s[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var from) &&
                float.TryParse(s[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var to))
                return new Range(from, to);

            throw new JsonSerializationException($"Converter cannot read {nameof(Range)} from JSON with the specified existing value");
        }
    }
}