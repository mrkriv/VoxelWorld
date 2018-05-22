using GameCore.Additional.Logging;

namespace GameCore.Services
{
    public class Config
    {
        public PathConfig Path { get; set; }
        public ChunkConfig Chunk { get; set; }
        public LoggerConfig LoggerConfig { get; set; } // todo: Сделать фабрику для получения конфигов

        public class PathConfig
        {
            private string _userInterface;
            private string _textures;
            private string _shaders;
            private string _font;

            public string Gamedata { get; set; }

            public string Textures
            {
                get => System.IO.Path.Combine(Gamedata, _textures);
                set => _textures = value;
            }

            public string Shaders
            {
                get => System.IO.Path.Combine(Gamedata, _shaders);
                set => _shaders = value;
            }

            public string UserInterface
            {
                get => System.IO.Path.Combine(Gamedata, _userInterface);
                set => _userInterface = value;
            }

            public string Font
            {
                get => System.IO.Path.Combine(Gamedata, _font);
                set => _font = value;
            }
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