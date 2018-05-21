using System;
using GameCore.EMath;
using GameCore.Render;
using GameCore.Render.Materials;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameApp.Entity
{
    public enum ChunkStatus
    {
        InvalidGenerated,
        ProcessGenerated,

        InvalidMesh,
        ProcessUpdateMesh,

        Active,
    }

    public class Chunk : GameCore.Entity.Entity
    {
        private BlockMaterial _material;
        private Texture _diffTexture;
        private Matrix4 _transform;
        private Block[,,] _map;
        private Mesh _mesh;

        public ChunkStatus Status { get; private set; }
        public int X { get; }
        public int Y { get; }

        public ChunkManager ChunkManager => ((VoxelWorld) World).ChunkManager;
        public Chunk FrontChunk => ChunkManager.GetChunk(X + 1, Y);
        public Chunk BackChunk => ChunkManager.GetChunk(X - 1, Y);
        public Chunk RightChunk => ChunkManager.GetChunk(X, Y + 1);
        public Chunk LeftChunk => ChunkManager.GetChunk(X, Y - 1);

        private int ChunkSizeH => World.Config.Chunk.ChunkSizeW;
        private int ChunkSizeV => World.Config.Chunk.ChunkSizeH;
        private int ChunkScale => World.Config.Chunk.ChunkScale;

        public Chunk(int x, int y)
        {
            Status = ChunkStatus.InvalidGenerated;
            X = x;
            Y = y;
        }

        public override void OnBeginPlay()
        {
            base.OnBeginPlay();

            _map = new Block[ChunkSizeH, ChunkSizeH, ChunkSizeV];

            _material = World.MaterialManager.Load<BlockMaterial>();
            _diffTexture = World.TextureManager.Load("terrain");

            _transform = Matrix4.CreateTranslation(X * ChunkSizeH, Y * ChunkSizeH, 0) *
                         Matrix4.CreateScale(ChunkScale);
        }

        public void Generated()
        {
            Status = ChunkStatus.ProcessGenerated;
            var rand = new Noise(10);

            var blockGrass = Block.FindByName("grass");
            var blockDirt = Block.FindByName("dirt");
            var blockRock = Block.FindByName("rock");
            var blockAdminium = Block.FindByName("adminium");

            for (var x = 0; x < ChunkSizeH; x++)
            {
                for (var y = 0; y < ChunkSizeH; y++)
                {
                    var noise = rand[(X * ChunkSizeH + x) / 50f, (Y * ChunkSizeH + y) / 50f];
                    var noise2 = rand[(X * ChunkSizeH + x) / 20f, (Y * ChunkSizeH + y) / 20f];

                    var groundLevel = (int) (ChunkSizeV * .5f + noise * ChunkSizeV * .2f);
                    var adminiumLevel = (int) (1 + noise2 * 3);

                    _map[x, y, groundLevel] = blockGrass;

                    for (var z = 0; z < groundLevel; z++)
                    {
                        if (z <= adminiumLevel)
                            _map[x, y, z] = blockAdminium;
                        else if (groundLevel - z < 5)
                            _map[x, y, z] = blockDirt;
                        else
                            _map[x, y, z] = blockRock;
                    }
                }
            }

            Status = ChunkStatus.InvalidMesh;
        }

        public void UpdateMesh()
        {
            Status = ChunkStatus.ProcessUpdateMesh;

            var vertexs = new Vector3[4 * 6 * ChunkSizeH * ChunkSizeH * ChunkSizeV];
            var normals = new Vector3[vertexs.Length];
            var texcood = new Vector2[vertexs.Length];

            var vertexsCount = 0;
            var normalsCount = 0;
            var texcoodCount = 0;
                
            for (var x = 0; x < ChunkSizeH; x++)
            {
                for (var y = 0; y < ChunkSizeH; y++)
                {
                    for (var z = 0; z < ChunkSizeV; z++)
                    {
                        var block = GetBlockLocalSpace(x, y, z);

                        if (block.Id == 0)
                            continue;

                        if (GetBlockLocalSpace(x, y, z - 1).Id == 0)
                        {
                            vertexs[vertexsCount++] = new Vector3(x, y, z); //buttom
                            vertexs[vertexsCount++] = new Vector3(x + 1, y, z);
                            vertexs[vertexsCount++] = new Vector3(x, y + 1, z);
                            vertexs[vertexsCount++] = new Vector3(x + 1, y + 1, z);

                            normals[normalsCount++] = new Vector3(0, 0, -1);
                            normals[normalsCount++] = new Vector3(0, 0, -1);
                            normals[normalsCount++] = new Vector3(0, 0, -1);
                            normals[normalsCount++] = new Vector3(0, 0, -1);

                            block.StaticData.TextureCoord.Buttom.CopyTo(texcood, ref texcoodCount);
                        }

                        if (GetBlockLocalSpace(x, y, z + 1).Id == 0)
                        {
                            vertexs[vertexsCount++] = new Vector3(x, y, z + 1); //top
                            vertexs[vertexsCount++] = new Vector3(x + 1, y, z + 1);
                            vertexs[vertexsCount++] = new Vector3(x, y + 1, z + 1);
                            vertexs[vertexsCount++] = new Vector3(x + 1, y + 1, z + 1);

                            normals[normalsCount++] = new Vector3(0, 0, 1);
                            normals[normalsCount++] = new Vector3(0, 0, 1);
                            normals[normalsCount++] = new Vector3(0, 0, 1);
                            normals[normalsCount++] = new Vector3(0, 0, 1);

                            block.StaticData.TextureCoord.Top.CopyTo(texcood, ref texcoodCount);
                        }

                        if (GetBlockLocalSpace(x, y - 1, z).Id == 0)
                        {
                            vertexs[vertexsCount++] = new Vector3(x, y, z); //right
                            vertexs[vertexsCount++] = new Vector3(x + 1, y, z);
                            vertexs[vertexsCount++] = new Vector3(x, y, z + 1);
                            vertexs[vertexsCount++] = new Vector3(x + 1, y, z + 1);

                            normals[normalsCount++] = new Vector3(0, -1, 0);
                            normals[normalsCount++] = new Vector3(0, -1, 0);
                            normals[normalsCount++] = new Vector3(0, -1, 0);
                            normals[normalsCount++] = new Vector3(0, -1, 0);

                            block.StaticData.TextureCoord.Right.CopyTo(texcood, ref texcoodCount);
                        }

                        if (GetBlockLocalSpace(x, y + 1, z).Id == 0)
                        {

                            vertexs[vertexsCount++] = new Vector3(x, y + 1, z); //left
                            vertexs[vertexsCount++] = new Vector3(x + 1, y + 1, z);
                            vertexs[vertexsCount++] = new Vector3(x, y + 1, z + 1);
                            vertexs[vertexsCount++] = new Vector3(x + 1, y + 1, z + 1);

                            normals[normalsCount++] = new Vector3(0, 1, 0);
                            normals[normalsCount++] = new Vector3(0, 1, 0);
                            normals[normalsCount++] = new Vector3(0, 1, 0);
                            normals[normalsCount++] = new Vector3(0, 1, 0);

                            block.StaticData.TextureCoord.Left.CopyTo(texcood, ref texcoodCount);
                        }

                        if (GetBlockLocalSpace(x + 1, y, z).Id == 0)
                        {

                            vertexs[vertexsCount++] = new Vector3(x + 1, y, z); //back
                            vertexs[vertexsCount++] = new Vector3(x + 1, y + 1, z);
                            vertexs[vertexsCount++] = new Vector3(x + 1, y, z + 1);
                            vertexs[vertexsCount++] = new Vector3(x + 1, y + 1, z + 1);

                            normals[normalsCount++] = new Vector3(1, 0, 0);
                            normals[normalsCount++] = new Vector3(1, 0, 0);
                            normals[normalsCount++] = new Vector3(1, 0, 0);
                            normals[normalsCount++] = new Vector3(1, 0, 0);

                            block.StaticData.TextureCoord.Back.CopyTo(texcood, ref texcoodCount);
                        }

                        if (GetBlockLocalSpace(x - 1, y, z).Id == 0)
                        {

                            vertexs[vertexsCount++] = new Vector3(x, y, z); //front
                            vertexs[vertexsCount++] = new Vector3(x, y + 1, z);
                            vertexs[vertexsCount++] = new Vector3(x, y, z + 1);
                            vertexs[vertexsCount++] = new Vector3(x, y + 1, z + 1);

                            normals[normalsCount++] = new Vector3(-1, 0, 0);
                            normals[normalsCount++] = new Vector3(-1, 0, 0);
                            normals[normalsCount++] = new Vector3(-1, 0, 0);
                            normals[normalsCount++] = new Vector3(-1, 0, 0);

                            block.StaticData.TextureCoord.Front.CopyTo(texcood, ref texcoodCount);
                        }
                    }
                }
            }

            Array.Resize(ref vertexs, vertexsCount);
            Array.Resize(ref normals, normalsCount);
            Array.Resize(ref texcood, texcoodCount);
            
            _mesh = new Mesh(vertexs, normals, texcood);
            Status = ChunkStatus.Active;
        }

        public Block GetBlockLocalSpace(int x, int y, int z)
        {
            if (z < 0 || z >= ChunkSizeV)
                return new Block();

            if (x < 0 || x >= ChunkSizeH || y < 0 || y >= ChunkSizeH)
            {
                return new Block();
                /*var chunkLocalX = x % ChunkSizeW;
                var chunkLocalY = y % ChunkSizeW;

                if (chunkLocalX < 0)
                    chunkLocalX = ChunkSizeW + chunkLocalX;

                if (chunkLocalY < 0)
                    chunkLocalY = ChunkSizeW + chunkLocalY;

                var chunkX = (x - chunkLocalX) / ChunkSizeW;
                var chunkY = (y - chunkLocalY) / ChunkSizeW;

                var chunk = World.ChunkManager.GetChunk(chunkX, chunkY);
                return chunk._map[chunkLocalX, chunkLocalY, z];*/
            }

            return _map[x, y, z];
        }

        public void SetBlock(Vector3 blockChunkPosition, Block block)
        {
            SetBlock((int) blockChunkPosition.X, (int) blockChunkPosition.Y, (int) blockChunkPosition.Z, block);
        }

        public void SetBlock(int x, int y, int z, Block block)
        {
            if (z < 0 || z >= ChunkSizeV)
                return;

            if (x < 0 || x >= ChunkSizeH || y < 0 || y >= ChunkSizeH)
                return;

            _map[x, y, z] = block;
            Status = ChunkStatus.InvalidMesh;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!Visiable)
                return;

            switch (Status)
            {
                case ChunkStatus.InvalidGenerated:
                case ChunkStatus.InvalidMesh:
                    ChunkManager.EnqueueChunkToUpdate(this);
                    break;
            }
        }

        public override void OnRender()
        {
            if (_mesh == null)
                return;

            base.OnRender();

            GL.ActiveTexture(TextureUnit.Texture0);

            _material.DiffTexture = _diffTexture;
            _material.Model = _transform;
            _material.Use();

            _mesh.Render(_material);
        }
    }
}