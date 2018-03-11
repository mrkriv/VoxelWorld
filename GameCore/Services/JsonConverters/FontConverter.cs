using System;
using GameCore.GUI;
using GameCore.Render;
using Newtonsoft.Json;

namespace GameCore.Services.JsonConverters
{
    public class FontConverter : JsonConverter<Font>
    {
        private readonly FontManager _fontManager;

        public FontConverter(FontManager fontManager)
        {
            _fontManager = fontManager;
        }

        public override void WriteJson(JsonWriter writer, Font value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override Font ReadJson(JsonReader reader, Type objectType, Font existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return _fontManager.Load((string) reader.Value);
        }
    }
}