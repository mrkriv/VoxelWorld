using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        public int ViewDistance => World.Config.Chunk.ViewDistance;

        public ChunkManager()
        {
            Name = "ChunkManager";
        }

        public override void OnBeginPlay()
        {
            base.OnBeginPlay();

            for (var i = 0; i < 3; i++)
            {
                var thread = new Thread(UpdateLoop)
                {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal
                };

                _updateThreads.Add(thread);
            }

            _threadsEnable = true;
            _updateThreads.ForEach(x => x.Start());
        }

        private void UpdateLoop()
        {
            while (_threadsEnable)
            {
                if (_updateQueue.Count == 0)
                {
                    Thread.Sleep(16);
                    continue;
                }

                _updateQueue.TryDequeue(out var chunk);

                if (chunk.Status == ChunkStatus.InvalidMesh)
                    chunk.UpdateMesh();
                else if (chunk.Status == ChunkStatus.InvalidGenerated)
                    chunk.Generated();
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
            _updateQueue.Enqueue(chunk);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            var origin = World.FindByName("Player").ChunkPosition;

            foreach (var chunk in _storage)
            {
                if (chunk.Visiable && (origin - new Vector2(chunk.X, chunk.Y)).LengthFast > ViewDistance)
                {
                    chunk.Visiable = false;
                }
            }

            for (var x = (int) origin.X - ViewDistance; x < origin.X + ViewDistance; x++)
            {
                for (var y = (int) origin.Y - ViewDistance; y < origin.Y + ViewDistance; y++)
                {
                    if ((origin - new Vector2(x, y)).LengthFast > ViewDistance)
                        continue;

                    var chunk = GetChunk(x, y);
                    chunk.Visiable = true;
                }
            }
        }

        private int ChunkSizeH => World.Config.Chunk.ChunkSizeW;
        private int ChunkSizeV => World.Config.Chunk.ChunkSizeH;
        private int ChunkScale => World.Config.Chunk.ChunkScale;

        public RayTraceResult RayTrace(Ray ray, int distance)
        {
            var point = ray.Origin;

            for (var i = 0; i < distance; i++)
            {
                if (point.Z > ChunkSizeV * ChunkScale || point.Z < 0)
                    return new RayTraceResult();

                var chunsPosX = (int) Math.Floor(point.X / ChunkSizeH / ChunkScale);
                var chunsPosY = (int) Math.Floor(point.Y / ChunkSizeH / ChunkScale);

                var chunk = GetChunk(chunsPosX, chunsPosY);

                var chunsLocalPosX = point.X / ChunkScale - chunsPosX * ChunkSizeH;
                var chunsLocalPosY = point.Y / ChunkScale - chunsPosY * ChunkSizeH;

                var block = chunk.GetBlockLocalSpace(
                    (int) Math.Floor(chunsLocalPosX),
                    (int) Math.Floor(chunsLocalPosY),
                    (int) Math.Floor(point.Z / ChunkScale));

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
                            MathF.Floor(point.Z / ChunkScale)),
                        BlockWorldPosition = new Vector3(
                            MathF.Floor(chunsLocalPosX) + chunsPosX * ChunkSizeH,
                            MathF.Floor(chunsLocalPosY) + chunsPosY * ChunkSizeH,
                            MathF.Floor(point.Z / ChunkScale)),
                    };
                }

                point += ray.Direction * ChunkScale;
            }

            return new RayTraceResult();
        }
    }
}