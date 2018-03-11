using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using GameCore.Render;
using OpenTK;

namespace GameCore.Entity
{
    public class ChunkManager : Entity
    {
        private readonly ConcurrentQueue<Chunk> _generateQueue = new ConcurrentQueue<Chunk>();
        private readonly List<Chunk> _storage = new List<Chunk>();
        private bool _generatorThreadEnable;
        private Thread _generatorThread;
        public int ViewDistance =>World.Config.Chunk.ViewDistance;

        public ChunkManager()
        {
            Name = "ChunkManager";
        }

        public override void OnBeginPlay()
        {
            base.OnBeginPlay();

            _generatorThread = new Thread(GeneratorLoop);
            _generatorThread.IsBackground = true;
            _generatorThread.Priority = ThreadPriority.BelowNormal;
            _generatorThreadEnable = true;

            _generatorThread.Start();
        }

        private void GeneratorLoop()
        {
            while (_generatorThreadEnable)
            {
                while (_generateQueue.Count != 0)
                {
                    _generateQueue.TryDequeue(out var chunk);

                    if (!chunk.IsLoaded && !chunk.IsLoadedStarted)
                        chunk.Load();
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _generatorThreadEnable = false;
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

            _generateQueue.Enqueue(chunk);
            _storage.Add(chunk);

            return chunk;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            var origin = World.FindByName("Player").ChunkPosition;

            foreach (var chunk in _storage)
            {
                if (chunk.IsVisiable && (origin - new Vector2(chunk.X, chunk.Y)).LengthFast > ViewDistance)
                {
                    chunk.IsVisiable = false;
                }
            }

            for (var x = (int) origin.X - ViewDistance; x < origin.X + ViewDistance; x++)
            {
                for (var y = (int) origin.Y - ViewDistance; y < origin.Y + ViewDistance; y++)
                {
                    if ((origin - new Vector2(x, y)).LengthFast > ViewDistance)
                        continue;

                    var chunk = GetChunk(x, y);

                    if (!chunk.IsLoaded && !chunk.IsLoadedStarted)
                        _generateQueue.Enqueue(chunk);

                    if (chunk.IsLoaded)
                        chunk.IsVisiable = true;
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