using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GameCore.Entity;
using GameCore.Render;
using GameCore.Render.Materials;
using GameCore.Services;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GameCore.GUI
{
    public class BindControlAttribute : Attribute
    {
    }
    
    public class Control
    {
        protected RootControl RootControl;

        public MaterialManager MaterialManager => RootControl.MaterialManager;
        public TextureManager TextureManager => RootControl.TextureManager;
        public InputManager InputManager => RootControl.InputManager;
        public FontManager FontManager => RootControl.FontManager;
        public World World => RootControl.World;

        public List<Control> Childs { get; set; } = new List<Control>();
        public Control Parrent { get; set; }
        public GuiVector Position { get; set; }
        public GuiVector Size { get; set; }
        public TextureCoord TextureCoord { get; set; }
        public Color4 Color { get; set; }
        public UserInterfaceMaterial Material { get; set; }
        public Texture Texture { get; set; }
        public bool IsVisiable { get; set; }
        public string Name { get; set; }

        public Control()
        {
            TextureCoord = new TextureCoord {W = 1, H = 1};
            IsVisiable = true;
        }
        
        public Control this[string name] => FindByName(name);
        
        public Control FindByName(string name)
        {
            //todo: make recursive
            return Childs.FirstOrDefault(x => x.Name == name);
        }

        public T FindByName<T>(string name) where  T : Control
        {
            return FindByName(name) as T;
        }

        public static Control AttachInFile(Control parrent, string file)
        {
            var cfg = parrent.RootControl.Config;
            var content = File.ReadAllText(Path.Combine(cfg.Path.UserInterface, file + ".json"));

            var control = JsonConvert.DeserializeObject<Control>(content);

            control.AttachTo(parrent);
            control.RecursiveAttachToSelf();
            control.Name = control.Name ?? file;
            control.BindControlsToProperty();
            
            return control;
        }

        private void BindControlsToProperty()
        {
            foreach (var prop in GetType().GetProperties())
            {
                var attr = prop.GetCustomAttribute<BindControlAttribute>();
                
                if(attr ==null)
                    continue;

                var name = prop.Name;
                var control = FindByName(name);

                if (control != null)
                {
                    prop.SetValue(this, control);
                }
                else
                {
                    Console.WriteLine($"Failed link control to property {name} in {GetType().Name}");
                }
            }
        }

        private void RecursiveAttachToSelf()
        {
            var childs = Childs.ToList();

            foreach (var child in childs)
            {
                child.AttachTo(this);
                child.RecursiveAttachToSelf();
            }

            Childs = childs;
        }

        public void AttachControl(Control child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            child.AttachTo(this);
        }

        public void AttachTo(Control parrent)
        {
            if (parrent == null)
                throw new ArgumentNullException(nameof(parrent));

            if (Parrent != null)
                throw new InvalidOperationException("Control aledaty attach");

            if (parrent.Parrent == null)
                throw new InvalidOperationException("Parent control is not attach");

            Parrent = parrent;
            parrent.Childs.Add(this);

            while (parrent is RootControl == false)
            {
                parrent = parrent?.Parrent;
            }

            RootControl = (RootControl) parrent;
            OnAttach();
        }

        public virtual void OnAttach()
        {
            Material = Material ?? Parrent.Material;
        }

        public virtual void OnTick(float dt)
        {
            Childs.ForEach(x => x.OnTick(dt));
        }

        public virtual void OnRender(Vector2 parrentPosition)
        {
            var position = parrentPosition + Position.ToVector2(this);
            var size = Size.ToVector2(this);
            
            if (Color.A > 0)
            {
                Material.Color = Color;
                Material.Transform = new Matrix3(
                    new Vector3(size.X, 0, 0),
                    new Vector3(0, size.Y, 0),
                    new Vector3(position.X, position.Y, 1));

                if (Texture != null)
                {
                    Material.UseTexture = true;
                    Material.Texture1 = Texture;
                    Material.TexcoodTransform = new Matrix3(
                        new Vector3(TextureCoord.W, 0, 0),
                        new Vector3(0, TextureCoord.H, 0),
                        new Vector3(TextureCoord.X, TextureCoord.Y, 1));
                }
                else
                {
                    Material.UseTexture = false;
                }

                Material.Use();

                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }

            foreach (var child in Childs)
            {
                if (child.IsVisiable)
                    child.OnRender(position);
            }
        }

        public override string ToString()
        {
            return Name ?? base.ToString();
        }
    }
}