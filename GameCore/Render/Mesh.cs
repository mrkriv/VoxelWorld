using System;
using GameCore.Render.Materials;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameCore.Render
{
    public class Mesh : IDisposable
    {
        private Vector3[] _vertexs;
        private Vector3[] _normals;
        private Vector2[] _texcood;
        private int _vertexsCount;
        private int _indexCount;
        private int[] _indices;
        private uint _vaoHandle;

        public Mesh(Vector3[] vertexs, Vector3[] normals, Vector2[] texcood, int vertexsCount = 0, int[] indices = null)
        {
            _vertexs = vertexs;
            _normals = normals;
            _texcood = texcood;
            _vertexsCount = vertexsCount != 0 ? vertexsCount : _vertexs.Length;
            _indices = indices ?? AutoBuildIndices();
        }

        private int[] AutoBuildIndices()
        {
            var indices = new int[_vertexsCount * 6 / 4];
            _indexCount = 0;

            for (var i = 0; i < _vertexsCount; i += 4)
            {
                indices[_indexCount++] = i + 0;
                indices[_indexCount++] = i + 1;
                indices[_indexCount++] = i + 2;

                indices[_indexCount++] = i + 2;
                indices[_indexCount++] = i + 3;
                indices[_indexCount++] = i + 1;
            }

            return indices;
        }

        private void UpdateVbo(MaterialBase material)
        {
            // VBO
            GL.GenBuffers(1, out uint vertexHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_vertexsCount * Vector3.SizeInBytes),
                _vertexs, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out uint normalsHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, normalsHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_vertexsCount * Vector3.SizeInBytes),
                _normals, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out uint texcoodHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texcoodHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(_vertexsCount * Vector2.SizeInBytes),
                _texcood, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out uint indexHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(sizeof(int) * _indices.Length),
                _indices, BufferUsageHint.StaticDraw);

            // VAO
            GL.GenVertexArrays(1, out _vaoHandle);
            GL.BindVertexArray(_vaoHandle);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
            material.BindInVertexPosition();

            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, normalsHandle);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
            material.BindInVertexNormal();

            GL.EnableVertexAttribArray(2);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texcoodHandle);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, true, Vector2.SizeInBytes, 0);
            material.BindInVertexTexcood();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexHandle);

            GL.BindVertexArray(0);

            _vertexs = null;
            _normals = null;
            _texcood = null;
            _indices = null;
        }
 
        public void Render(Block material)
        {
            if (_vertexs != null)
                UpdateVbo(material);

            GL.BindVertexArray(_vaoHandle);
            GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public void Dispose()
        {
            if(_vaoHandle > 0)
                GL.DeleteVertexArray(_vaoHandle);
        }
    }
}