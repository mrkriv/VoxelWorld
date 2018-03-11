namespace GameCore.Services
{
    public class Config
    {
        public PathConfig Path { get; set; }
        public ChunkConfig Chunk { get; set; }

        public class PathConfig
        {
            public string Gamedata { get; set; }
            public string Textures { get; set; }
            public string Shaders { get; set; }
            public string UserInterface { get; set; }
            public string Font { get; set; }
        }

        public class ChunkConfig
        {
            public int ChunkSizeW { get; set; }
            public int ChunkSizeH { get; set; }
            public int ChunkScale { get; set; }
            public int ViewDistance { get; set; }
        }
    }
}