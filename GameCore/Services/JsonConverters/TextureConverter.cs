using System;
using GameCore.Render;
using Newtonsoft.Json;

namespace GameCore.Services.JsonConverters
{
    public class TextureConverter : JsonConverter<Texture>
    {
        private readonly TextureManager _textureManager;

        public TextureConverter(TextureManager textureManager)
        {
            _textureManager = textureManager;
        }

        public override void WriteJson(JsonWriter writer, Texture value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override Texture ReadJson(JsonReader reader, Type objectType, Texture existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return _textureManager.Load((string) reader.Value);
        }
    }
}