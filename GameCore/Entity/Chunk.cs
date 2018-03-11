using System;
using GameCore.Render;
using GameCore.Render.Materials;
using GameCore.Services;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameCore.Entity
{
    public class Chunk : Entity
    {
        private uint _vaoHandle;
        private int _indexCount;
        private int _vertexsCount;
        private int _normalsCount;
        private int _texcoodCount;
        private BlockMaterial _material;
        private Block[,,] _map;

        public bool IsLoadedStarted { get; private set; }
        public bool IsGenerated { get; private set; }
        public bool IsLoaded { get; private set; }
        public int X { get; }
        public int Y { get; }

        public Chunk FrontChunk => World.ChunkManager.GetChunk(X + 1, Y);
        public Chunk BackChunk => World.ChunkManager.GetChunk(X - 1, Y);
        public Chunk RightChunk => World.ChunkManager.GetChunk(X, Y + 1);
        public Chunk LeftChunk => World.ChunkManager.GetChunk(X, Y - 1);

        private int ChunkSizeH => World.Config.Chunk.ChunkSizeW;
        private int ChunkSizeV => World.Config.Chunk.ChunkSizeH;
        private int ChunkScale => World.Config.Chunk.ChunkScale;

        private Texture _diffTexture;
        private Matrix4 _transform;
        private Vector3[] _vertexs;
        private Vector3[] _normals;
        private Vector2[] _texcood;
        private int[] _indices;

        public Chunk(int x, int y)
        {
            X = x;
            Y = y;
            IsVisiable = false;
            IsLoaded = false;
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

        public void Load()
        {
            IsLoadedStarted = true;

            if (!IsGenerated)
                Generated();

            UpdateMesh();
            IsLoaded = true;
        }

        public void Generated()
        {
            var rand = new Rand(10);

            var blockGrass = Block.FindByName("grass");
            var blockDirt = Block.FindByName("dirt");
            var blockRock = Block.FindByName("rock");
            var blockAdminium = Block.FindByName("adminium");

            for (var x = 0; x < ChunkSizeH; x++)
            {
                for (var y = 0; y < ChunkSizeH; y++)
                {
                    var noise = rand.Noise((X * ChunkSizeH + x) / 50f, (Y * ChunkSizeH + y) / 50f);
                    var noise2 = rand.Noise((X * ChunkSizeH + x) / 20f, (Y * ChunkSizeH + y) / 20f);

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

            IsGenerated = true;
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

        public void UpdateMesh()
        {
            _indexCount = 0;
            _vertexsCount = 0;
            _normalsCount = 0;
            _texcoodCount = 0;

            _indices = new int[6 * 6 * ChunkSizeH * ChunkSizeH * ChunkSizeV];
            _vertexs = new Vector3[4 * 6 * ChunkSizeH * ChunkSizeH * ChunkSizeV];
            _normals = new Vector3[_vertexs.Length];
            _texcood = new Vector2[_vertexs.Length];

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
                            _vertexs[_vertexsCount++] = new Vector3(x, y, z); //buttom
                            _vertexs[_vertexsCount++] = new Vector3(x + 1, y, z);
                            _vertexs[_vertexsCount++] = new Vector3(x, y + 1, z);
                            _vertexs[_vertexsCount++] = new Vector3(x + 1, y + 1, z);

                            _normals[_normalsCount++] = new Vector3(0, 0, -1);
                            _normals[_normalsCount++] = new Vector3(0, 0, -1);
                            _normals[_normalsCount++] = new Vector3(0, 0, -1);
                            _normals[_normalsCount++] = new Vector3(0, 0, -1);

                            block.StaticData.TextureCoord.Buttom.CopyTo(_texcood, ref _texcoodCount);
                        }

                        if (GetBlockLocalSpace(x, y, z + 1).Id == 0)
                        {
                            _vertexs[_vertexsCount++] = new Vector3(x, y, z + 1); //top
                            _vertexs[_vertexsCount++] = new Vector3(x + 1, y, z + 1);
                            _vertexs[_vertexsCount++] = new Vector3(x, y + 1, z + 1);
                            _vertexs[_vertexsCount++] = new Vector3(x + 1, y + 1, z + 1);

                            _normals[_normalsCount++] = new Vector3(0, 0, 1);
                            _normals[_normalsCount++] = new Vector3(0, 0, 1);
                            _normals[_normalsCount++] = new Vector3(0, 0, 1);
                            _normals[_normalsCount++] = new Vector3(0, 0, 1);

                            block.StaticData.TextureCoord.Top.CopyTo(_texcood, ref _texcoodCount);
                        }

                        if (GetBlockLocalSpace(x, y - 1, z).Id == 0)
                        {
                            _vertexs[_vertexsCount++] = new Vector3(x, y, z); //right
                            _vertexs[_vertexsCount++] = new Vector3(x + 1, y, z);
                            _vertexs[_vertexsCount++] = new Vector3(x, y, z + 1);
                            _vertexs[_vertexsCount++] = new Vector3(x + 1, y, z + 1);


                            _normals[_normalsCount++] = new Vector3(0, -1, 0);
                            _normals[_normalsCount++] = new Vector3(0, -1, 0);
                            _normals[_normalsCount++] = new Vector3(0, -1, 0);
                            _normals[_normalsCount++] = new Vector3(0, -1, 0);

                            block.StaticData.TextureCoord.Right.CopyTo(_texcood, ref _texcoodCount);
                        }

                        if (GetBlockLocalSpace(x, y + 1, z).Id == 0)
                        {

                            _vertexs[_vertexsCount++] = new Vector3(x, y + 1, z); //left
                            _vertexs[_vertexsCount++] = new Vector3(x + 1, y + 1, z);
                            _vertexs[_vertexsCount++] = new Vector3(x, y + 1, z + 1);
                            _vertexs[_vertexsCount++] = new Vector3(x + 1, y + 1, z + 1);

                            _normals[_normalsCount++] = new Vector3(0, 1, 0);
                            _normals[_normalsCount++] = new Vector3(0, 1, 0);
                            _normals[_normalsCount++] = new Vector3(0, 1, 0);
                            _normals[_normalsCount++] = new Vector3(0, 1, 0);

                            block.StaticData.TextureCoord.Left.CopyTo(_texcood, ref _texcoodCount);
                        }

                        if (GetBlockLocalSpace(x + 1, y, z).Id == 0)
                        {

                            _vertexs[_vertexsCount++] = new Vector3(x + 1, y, z); //back
                            _vertexs[_vertexsCount++] = new Vector3(x + 1, y + 1, z);
                            _vertexs[_vertexsCount++] = new Vector3(x + 1, y, z + 1);
                            _vertexs[_vertexsCount++] = new Vector3(x + 1, y + 1, z + 1);


                            _normals[_normalsCount++] = new Vector3(1, 0, 0);
                            _normals[_normalsCount++] = new Vector3(1, 0, 0);
                            _normals[_normalsCount++] = new Vector3(1, 0, 0);
                            _normals[_normalsCount++] = new Vector3(1, 0, 0);

                            block.StaticData.TextureCoord.Back.CopyTo(_texcood, ref _texcoodCount);
                        }

                        if (GetBlockLocalSpace(x - 1, y, z).Id == 0)
                        {

                            _vertexs[_vertexsCount++] = new Vector3(x, y, z); //front
                            _vertexs[_vertexsCount++] = new Vector3(x, y + 1, z);
                            _vertexs[_vertexsCount++] = new Vector3(x, y, z + 1);
                            _vertexs[_vertexsCount++] = new Vector3(x, y + 1, z + 1);


                            _normals[_normalsCount++] = new Vector3(-1, 0, 0);
                            _normals[_normalsCount++] = new Vector3(-1, 0, 0);
                            _normals[_normalsCount++] = new Vector3(-1, 0, 0);
                            _normals[_normalsCount++] = new Vector3(-1, 0, 0);

                            block.StaticData.TextureCoord.Front.CopyTo(_texcood, ref _texcoodCount);
                        }
                    }
                }
            }

            for (var i = 0; i < _vertexsCount; i += 4)
            {
                _indices[_indexCount++] = i + 0;
                _indices[_indexCount++] = i + 1;
                _indices[_indexCount++] = i + 2;

                _indices[_indexCount++] = i + 2;
                _indices[_indexCount++] = i + 3;
                _indices[_indexCount++] = i + 1;
            }
        }

        public void UpdateVbo()
        {
            // VBO
            GL.GenBuffers(1, out uint vertexHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexHandle);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(_vertexsCount * Vector3.SizeInBytes),
                _vertexs, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out uint normalsHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, normalsHandle);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(_normalsCount * Vector3.SizeInBytes),
                _normals, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out uint texcoodHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texcoodHandle);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(_texcoodCount * Vector2.SizeInBytes),
                _texcood, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out uint indexHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                new IntPtr(sizeof(int) * _indexCount),
                _indices, BufferUsageHint.StaticDraw);

            // VAO
            GL.GenVertexArrays(1, out _vaoHandle);
            GL.BindVertexArray(_vaoHandle);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
            _material.BindInVertexPosition();

            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, normalsHandle);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
            _material.BindInVertexNormal();

            GL.EnableVertexAttribArray(2);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texcoodHandle);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, true, Vector2.SizeInBytes, 0);
            _material.BindInVertexTexcood();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexHandle);

            GL.BindVertexArray(0);

            _indices = null;
            _vertexs = null;
            _normals = null;
            _texcood = null;
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
            IsVisiable = false;
            IsLoaded = false;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (IsLoaded && IsLoadedStarted)
            {
                UpdateVbo();
                IsLoadedStarted = false;
                IsVisiable = true;
            }
        }

        public override void OnRender()
        {
            base.OnRender();

            GL.ActiveTexture(TextureUnit.Texture0);

            _material.DiffTexture = _diffTexture;
            _material.Model = _transform;
            _material.Use();

            GL.BindVertexArray(_vaoHandle);
            GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}