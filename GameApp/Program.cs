using System;
using System.IO;
using System.Text;
using GameCore.Entity;
using GameCore.GUI;
using GameCore.Render;
using GameCore.Services;
using GameLogic.Entity.Global;
using Newtonsoft.Json;

namespace GameApp
{
    class Game
    {
        [STAThread]
        static void Main()
        {
            var di = new DependencyInjection();

            di.AddSinglton(JsonConvert.DeserializeObject<Config>(File.ReadAllText("appconfig.json", Encoding.UTF8)));
            
            di.AddSinglton<Viewport>();
            di.AddSinglton<InputManager>();
            di.AddSinglton<RootControl>();
            di.AddSinglton<MaterialManager>();
            di.AddSinglton<TextureManager>();
            di.AddSinglton<FontManager>();
            di.AddTransient<World, TestWorld>();

            var game = di.GetService<Viewport>();
            game.Run();
        }
    }
}