using System;
using System.Collections.Generic;
using OpenTK;

namespace GameCore.Entity
{
    public abstract class Entity
    {
        public readonly List<Entity> Childrens = new List<Entity>();
        public World World { get; set; }

        public bool Visiable { get; set; } = true;
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Entity Parrent { get; set; }
        public string Name { get; set; }

        public Vector3 Forward => new Vector3
        {
            X = MathF.Sin(Rotation.X) * MathF.Cos(Rotation.Y),
            Y = MathF.Cos(Rotation.X) * MathF.Cos(Rotation.Y),
            Z = MathF.Sin(Rotation.Y)
        };

        public Vector3 Right => new Vector3
        {
            X = MathF.Sin(Rotation.X - MathF.PI / 2) * MathF.Cos(Rotation.Y),
            Y = MathF.Cos(Rotation.X - MathF.PI / 2) * MathF.Cos(Rotation.Y),
            Z = MathF.Sin(Rotation.Y)
        };

        public Vector3 Up => new Vector3
        {
            X = MathF.Sin(Rotation.X) * MathF.Cos(Rotation.Y + MathF.PI / 2),
            Y = MathF.Cos(Rotation.X) * MathF.Cos(Rotation.Y + MathF.PI / 2),
            Z = MathF.Sin(Rotation.Y + MathF.PI / 2)
        };

        public Vector2 ChunkPosition => new Vector2
        {
            X = MathF.Floor(Position.X / World.Config.Chunk.ChunkSizeW / World.Config.Chunk.ChunkScale),
            Y = MathF.Floor(Position.Y / World.Config.Chunk.ChunkSizeW / World.Config.Chunk.ChunkScale)
        };
        
        public Vector3 ChunkSpacePosition => new Vector3
        {
            X = MathF.Floor(Position.X / World.Config.Chunk.ChunkScale),
            Y = MathF.Floor(Position.Y / World.Config.Chunk.ChunkScale),
            Z = MathF.Floor(Position.Z / World.Config.Chunk.ChunkScale)
        };

        public void AttachTo(Entity obj)
        {
            if (Parrent == obj)
                return;

            if (Parrent != null)
            {
                //todo: ...
            }

            Parrent = obj;
            World = Parrent.World;
            Parrent.Childrens.Add(this);

            OnBeginPlay();
        }

        public virtual void OnBeginPlay()
        {
            Childrens.ForEach(x => x.OnBeginPlay());
        }

        public virtual void OnTick(float dt)
        {
            Childrens.ForEach(x => x.OnTick(dt));
        }

        public virtual void OnRender()
        {
            foreach (var obj in Childrens)
            {
                if (obj.Visiable)
                    obj.OnRender();
            }
        }

        public virtual void OnDestroy()
        {
            Childrens.ForEach(x => x.OnDestroy());
        }
    }
}