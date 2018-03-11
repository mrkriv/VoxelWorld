using GameCore.Entity;
using OpenTK;

namespace GameCore.Render
{
    public class RayTraceResult
    {
        public Vector3 Position { get; set; }
        public Vector3 BlockWorldPosition { get; set; }
        public Vector3 BlockChunkPosition { get; set; }
        public Block Block { get; set; }
        public Chunk Chunk { get; set; }
    }
}