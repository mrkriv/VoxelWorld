using GameApp.Entity;
using OpenTK;

namespace GameApp.EMath
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