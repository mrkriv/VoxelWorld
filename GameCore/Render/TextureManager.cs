﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GameCore.Additional.Logging;
using GameCore.Services;

namespace GameCore.Render
{
    public class TextureManager
    {
        private readonly Dictionary<string, Texture> _storage = new Dictionary<string, Texture>();
        private readonly Logger<TextureManager> _logger;
        private readonly Config _config;

        public TextureManager(Config config, Logger<TextureManager> logger)
        {
            _config = config;
            _logger = logger;
        }

        public Texture Load(string name)
        {
            if (_storage.ContainsKey(name))
                return _storage[name];

            var path = Path.Combine(_config.Path.Textures, name + ".png");
            
            if (!File.Exists(path))
            {
                _logger.Error($"File not found {name}");
                return null;
            }

            using (var bitmap = new Bitmap(path))
            {
                var texture = new Texture(name, bitmap);
                _storage.Add(name, texture);

                _logger.Log($"Load '{name}'");
                return texture;
            }
        }
    }
}