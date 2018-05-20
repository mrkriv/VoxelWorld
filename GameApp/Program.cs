﻿using System;
using System.IO;
using System.Text;
using GameApp.Entity.Global;
using GameApp.Services;
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
            var di = new DependencyInjection();

            di.AddSinglton(JsonConvert.DeserializeObject<Config>(File.ReadAllText("appconfig.json", Encoding.UTF8)));
            
            di.AddSinglton<AppWindow, VoxelWorldWindow>();
            di.AddSinglton<InputManager>();
            di.AddSinglton<RootControl>();
            di.AddSinglton<MaterialManager>();
            di.AddSinglton<TextureManager>();
            di.AddSinglton<FontManager>();
            di.AddTransient<World, VoxelWorld>();

            var game = di.GetService<AppWindow>();
            game.Run();
        }
    }
}