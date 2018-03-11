using GameCore.Entity;
using GameCore.GUI;
using GameCore.Render;
using GameCore.Services;
using GameLogic.Entity.Characters;

namespace GameLogic.Entity.Global
{
    public class TestWorld : World
    {
        public TestWorld(
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
            //RootControl.AttachControl(new CursorControl());
        }
    }
}