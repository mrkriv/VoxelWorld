using System.Collections.Generic;
using System.Linq;
using GameCore.GUI;
using GameCore.Render;
using GameCore.Services;

namespace GameCore.Entity
{
    public class World
    {
        private readonly List<Entity> _gameObjects = new List<Entity>();

        public ChunkManager ChunkManager { get; protected set; }
        public InputManager InputManager { get; }
        public RootControl RootControl { get; }
        public MaterialManager MaterialManager { get; }
        public TextureManager TextureManager { get; }
        public Config Config { get; }

        public World(
            InputManager inputManager,
            RootControl rootControl,
            MaterialManager materialManager,
            TextureManager textureManager,
            Config config)
        {
            InputManager = inputManager;
            RootControl = rootControl;
            MaterialManager = materialManager;
            TextureManager = textureManager;
            Config = config;
        }

        public Entity this[string name] => FindByName(name);
        
        public Entity FindByName(string name)
        {
            return _gameObjects.FirstOrDefault(x => x.Name == name);
        }

        public T FindByName<T>(string name) where T : Entity
        {
            return FindByName(name) as T;
        }

        public virtual void OnLoad()
        {
            
        }

        public virtual void AtachObjectToWorld(Entity obj)
        {
            var needInvBeginPlay = obj.World == null;
            
            if (obj.World == this)
                return;

            obj.World?._gameObjects.Remove(obj);
            _gameObjects.Add(obj);

            obj.World = this;
            
            if (needInvBeginPlay)
                obj.OnBeginPlay();
        }

        public virtual void OnTick(float dt)
        {
            _gameObjects.ForEach(x => x.OnTick(dt));
        }

        public virtual void OnRender()
        {
            _gameObjects.ForEach(c => c.OnRender());
        }

        public virtual void OnDestroy()
        {
            _gameObjects.ForEach(c => c.OnDestroy());
        }
    }
}