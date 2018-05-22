using System;
using System.IO;
using System.Text;
using GameApp.Entity;
using GameApp.Services;
using GameCore.Additional.Logging;
using GameCore.Entity;
using GameCore.GUI;
using GameCore.Render;
using GameCore.Services;
using Newtonsoft.Json;

namespace GameApp
{
    class Game
    {
        [STAThread]
        static void Main()
        {
            var di = new ServiceProvider();
            var cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText("appconfig.json", Encoding.UTF8));

            di.AddSinglton<AppWindow, VoxelWorldWindow>();
            di.AddTransient<World, VoxelWorld>();
            di.AddSinglton<InputManager>();
            di.AddSinglton<RootControl>();
            di.AddSinglton<MaterialManager>();
            di.AddSinglton<TextureManager>();
            di.AddSinglton<FontManager>();
            di.AddSinglton<Logger>();
            di.AddSinglton(cfg);

            var game = di.GetService<AppWindow>();
            game.Run();
        }
    }
}