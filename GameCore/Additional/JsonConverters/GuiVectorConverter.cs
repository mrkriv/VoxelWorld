using System;
using System.Globalization;
using GameCore.EMath;
using Newtonsoft.Json;

namespace GameCore.Additional.JsonConverters
{
    public class GuiVectorConverter : JsonConverter<GuiVector>
    {
        public override void WriteJson(JsonWriter writer, GuiVector value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override GuiVector ReadJson(JsonReader reader, Type objectType, GuiVector existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var s = ((string) reader.Value).Split(' ');

            if (s.Length == 3 &&
                Enum.TryParse<GuiVectorType>(s[0], out var type) &&
                float.TryParse(s[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var x) &&
                float.TryParse(s[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
                return new GuiVector(type, x, y);

            throw new JsonSerializationException($"Converter cannot read {nameof(GuiVector)} from JSON with the specified existing value");
        }
    }
}