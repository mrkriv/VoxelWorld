using System.Collections.Generic;
using System.IO;
using GameCore.Services;

namespace GameCore.GUI
{
    public class FontManager
    {
        private readonly Dictionary<string, Font> _storage = new Dictionary<string, Font>();
        private readonly Config _config;

        public FontManager(Config config)
        {
            _config = config;
        }

        public Font Load(string name)
        {
            if (_storage.ContainsKey(name))
                return _storage[name];

            var path = Path.Combine(_config.Path.Font, name);

            var texture = new Font(path);
            _storage.Add(name, texture);

            return texture;
        }
    }
}