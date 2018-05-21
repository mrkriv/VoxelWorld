using GameApp.Entity.Characters;
using GameCore.Entity;
using GameCore.GUI;
using GameCore.Render;
using GameCore.Services;

namespace GameApp.Entity
{
    public class VoxelWorld : World
    {
        public ChunkManager ChunkManager { get; protected set; }
        
        public VoxelWorld(
            InputManager inputManager,
            RootControl rootControl,
            MaterialManager materialManager,
            TextureManager textureManager,
            Config config)
            : base(inputManager, rootControl, materialManager, textureManager, config)
        {
        }

        public override void OnLoad()
        {
            RootControl.OnAttach(this);
            
            ChunkManager = new ChunkManager();
            AtachObjectToWorld(ChunkManager);
            AtachObjectToWorld(new Player());
            AtachObjectToWorld(new Grid());
            
            Control.AttachInFile(RootControl, "debug_menu");
            RootControl.AttachControl(new CursorControl());
        }
    }
}