using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameApp.EMath;
using GameCore.EMath;
using OpenTK;

namespace GameApp.Entity
{
    public class ChunkManager : GameCore.Entity.Entity
    {
        private readonly ConcurrentQueue<Chunk> _updateQueue = new ConcurrentQueue<Chunk>();
        private readonly List<Chunk> _storage = new List<Chunk>();
        private readonly List<Thread> _updateThreads = new List<Thread>();
        private bool _threadsEnable;

        public ChunkManager()
        {
            Name = "ChunkManager";
        }

        public override void OnBeginPlay()
        {
            base.OnBeginPlay();

            for (var i = 0; i < 3; i++)
            {
                _updateThreads.Add(new Thread(UpdateLoop)
                {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal
                });
            }

            _threadsEnable = true;
            _updateThreads.ForEach(x => x.Start());
        }

        private void UpdateLoop()
        {
            while (_threadsEnable)
            {
                while (_updateQueue.TryDequeue(out var chunk))
                {
                    if (chunk.Status == ChunkStatus.InvalidMesh)
                        chunk.UpdateMesh();
                }
                Thread.Sleep(10);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _threadsEnable = false;
        }

        public Chunk GetChunk(int x, int y)
        {
            foreach (var chunk in _storage)
            {
                if (chunk.X == x && chunk.Y == y)
                {
                    return chunk;
                }
            }

            return CreateChunk(x, y);
        }

        private Chunk CreateChunk(int x, int y)
        {
            var chunk = new Chunk(x, y);
            chunk.AttachTo(this);

            _updateQueue.Enqueue(chunk);
            _storage.Add(chunk);

            return chunk;
        }

        public void EnqueueChunkToUpdate(Chunk chunk)
        {
            if (!_updateQueue.Contains(chunk))
                _updateQueue.Enqueue(chunk);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            var origin = World.FindByName("Player").ChunkPosition;
            var viewDistance = World.Config.Chunk.ViewDistance;

            foreach (var chunk in _storage)
            {
                if (chunk.Visiable && (origin - new Vector2(chunk.X, chunk.Y)).LengthFast > viewDistance)
                {
                    chunk.Visiable = false;
                }
            }
            
            for (var x = (int) origin.X - viewDistance; x < origin.X + viewDistance; x++)
            {
                for (var y = (int) origin.Y - viewDistance; y < origin.Y + viewDistance; y++)
                {
                    if ((origin - new Vector2(x, y)).LengthFast > viewDistance)
                        continue;

                    var chunk = GetChunk(x, y);
                    chunk.Visiable = true;
                }
            }
        }

        public RayTraceResult RayTrace(Ray ray, int distance)
        {
            var chunkSizeH = World.Config.Chunk.ChunkSizeW;
            var chunkSizeV = World.Config.Chunk.ChunkSizeH;
            var chunkScale = World.Config.Chunk.ChunkScale;
            var point = ray.Origin;

            for (var i = 0; i < distance; i++)
            {
                if (point.Z > chunkSizeV * chunkScale || point.Z < 0)
                    return new RayTraceResult();

                var chunsPosX = (int) Math.Floor(point.X / chunkSizeH / chunkScale);
                var chunsPosY = (int) Math.Floor(point.Y / chunkSizeH / chunkScale);

                var chunk = GetChunk(chunsPosX, chunsPosY);

                var chunsLocalPosX = point.X / chunkScale - chunsPosX * chunkSizeH;
                var chunsLocalPosY = point.Y / chunkScale - chunsPosY * chunkSizeH;

                var block = chunk.GetBlockLocalSpace(
                    (int) Math.Floor(chunsLocalPosX),
                    (int) Math.Floor(chunsLocalPosY),
                    (int) Math.Floor(point.Z / chunkScale));

                if (block.Id != 0)
                {
                    return new RayTraceResult
                    {
                        Position = point,
                        Block = block,
                        Chunk = chunk,
                        BlockChunkPosition = new Vector3(
                            MathF.Floor(chunsLocalPosX),
                            MathF.Floor(chunsLocalPosY),
                            MathF.Floor(point.Z / chunkScale)),
                        BlockWorldPosition = new Vector3(
                            MathF.Floor(chunsLocalPosX) + chunsPosX * chunkSizeH,
                            MathF.Floor(chunsLocalPosY) + chunsPosY * chunkSizeH,
                            MathF.Floor(point.Z / chunkScale)),
                    };
                }

                point += ray.Direction * chunkScale;
            }

            return new RayTraceResult();
        }
    }
}