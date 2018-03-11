using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using GameCore.GUI;
using GameCore.Services;

namespace GameCore.Render
{
    public class TextureManager
    {
        private readonly Dictionary<string, Texture> _storage = new Dictionary<string, Texture>();
        private readonly Config _config;

        public TextureManager(Config config)
        {
            _config = config;
        }

        public Texture Load(string name)
        {
            if (_storage.ContainsKey(name))
                return _storage[name];

            var path = Path.Combine(_config.Path.Textures, name + ".png");

            using (var bitmap = new Bitmap(path))
            {
                var texture = new Texture(name, bitmap);
                _storage.Add(name, texture);

                return texture;
            }
        }
    }
}