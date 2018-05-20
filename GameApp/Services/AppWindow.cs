using System;
using GameApp.Entity;
using GameCore.Entity;
using GameCore.GUI;
using GameCore.Render;
using GameCore.Services;

namespace GameApp.Services
{
    public class VoxelWorldWindow : AppWindow
    {
        public VoxelWorldWindow(
            World world,
            FontManager fontManager,
            RootControl rootControl,
            InputManager inputManager,
            TextureManager textureManager,
            MaterialManager materialManager)
            : base(world, fontManager, rootControl, inputManager, textureManager, materialManager)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            Block.RegStandartBlocks();
            base.OnLoad(e);
        }
    }
}