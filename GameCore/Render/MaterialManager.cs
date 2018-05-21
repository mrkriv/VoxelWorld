using System;
using System.Collections.Generic;
using System.IO;
using GameCore.Render.Materials;
using GameCore.Services;

namespace GameCore.Render
{
    public class MaterialManager
    {
        private readonly Dictionary<string, MaterialBase> _storage = new Dictionary<string, MaterialBase>();
        private readonly Config _config;

        public MaterialManager(Config config)
        {
            _config = config;
        }

        public T Load<T>() where T : MaterialBase
        {
            var defaultNameProp = typeof(T).GetProperty(nameof(MaterialBase.DefaultShaderName));

            if (defaultNameProp == null)
                throw new NotSupportedException($"Material type {typeof(T).Name} is not support default name");

            var defaultName = defaultNameProp.GetValue(null) as string;

            return Load<T>(defaultName);
        }

        public T Load<T>(string name) where T : MaterialBase
        {
            if (_storage.ContainsKey(name))
                return _storage[name] as T;

            var fs = Path.Combine(_config.Path.Shaders, name, "fs.glsl");
            var vs = Path.Combine(_config.Path.Shaders, name, "vs.glsl");

            var mtl = Activator.CreateInstance(typeof(T), fs, vs) as MaterialBase;    //todo: поменять на что то более производительное
            _storage.Add(name, mtl);

            return mtl as T;
        }

        public void AppyGlobal()
        {
            foreach (var kpv in _storage)
            {
                kpv.Value.AppyGlobal();
            }
        }
    }
}