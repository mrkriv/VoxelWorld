using System.Collections.Generic;
using System.IO;
using GameCore.Additional.Logging;
using GameCore.Services;

namespace GameCore.GUI
{
    public class FontManager
    {
        private readonly Dictionary<string, Font> _storage = new Dictionary<string, Font>();
        private readonly Logger<FontManager> _logger;
        private readonly Config _config;

        public FontManager(Config config, Logger<FontManager> logger)
        {
            _config = config;
            _logger = logger;
        }

        public Font Load(string name)
        {
            if (_storage.ContainsKey(name))
                return _storage[name];

            var path = Path.Combine(_config.Path.Font, name);
            if (!File.Exists(path + ".json")) // todo: запихать логгер в Font
            {
                _logger.Error($"File not found {path + ".json"}");
                return null;
            }

            var texture = new Font(path);
            _storage.Add(name, texture);

            _logger.Log($"Load '{name}'");
            return texture;
        }
    }
}