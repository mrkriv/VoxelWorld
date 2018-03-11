using System;
using System.Globalization;
using Newtonsoft.Json;
using OpenTK.Graphics;

namespace GameCore.Services.JsonConverters
{
    public class ColorConverter : JsonConverter<Color4>
    {
        public override void WriteJson(JsonWriter writer, Color4 value, JsonSerializer serializer)
        {
            writer.WriteValue($"{value.R} {value.G} {value.B} {value.A}");
        }

        public override Color4 ReadJson(JsonReader reader, Type objectType, Color4 existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var s = ((string) reader.Value).Split(' ');

            if (s.Length >= 3 &&
                float.TryParse(s[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var r) &&
                float.TryParse(s[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var g) &&
                float.TryParse(s[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var b))
            {
                var a = 1f;

                if (s.Length == 4)
                    float.TryParse(s[3].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out a);

                return new Color4(r, g, b, a);
            }

            throw new JsonSerializationException($"Converter cannot read {nameof(Color4)} from JSON with the specified existing value");
        }
    }
}