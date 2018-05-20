using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GameCore.Render.Materials
{
    public class MateriaParamAttribute : Attribute
    {
    }

    public abstract class BaseMaterial
    {
        private readonly Dictionary<PropertyInfo, int> _paramLinks = new Dictionary<PropertyInfo, int>();
        protected readonly int Handle;
        
        public static string DefaultShaderName => null;

        public abstract void BindInVertexPosition();
        public abstract void BindInVertexNormal();
        public abstract void BindInVertexColor();
        public abstract void BindInVertexTexcood();

        public BaseMaterial(string fsPath, string vsPath)
        {
            var fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            var vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            
            GL.ShaderSource(fragmentShaderHandle, File.ReadAllText(fsPath, Encoding.UTF8));
            GL.ShaderSource(vertexShaderHandle, File.ReadAllText(vsPath, Encoding.UTF8));
            
            GL.CompileShader(vertexShaderHandle);
            GL.CompileShader(fragmentShaderHandle);

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vertexShaderHandle);
            GL.AttachShader(Handle, fragmentShaderHandle);

            GL.LinkProgram(Handle);
            LinkProps();

            var log = GL.GetProgramInfoLog(Handle);
            if (!string.IsNullOrWhiteSpace(log))
                Console.WriteLine(log);
        }

        private void LinkProps()
        {
            var type = GetType();

            foreach (var property in type.GetProperties())
            {
                var matParamAttr = property.GetCustomAttribute<MateriaParamAttribute>();
                if (matParamAttr == null)
                    continue;

                if (property.PropertyType != typeof(Texture))
                {
                    var name = char.ToLower(property.Name[0]) + property.Name.Substring(1);
                    var location = GL.GetUniformLocation(Handle, name);

                    if (location == -1)
                        Console.WriteLine($"Failed link property {name} to shader in {GetType().Name}");

                    _paramLinks.Add(property, location);
                }
                else
                {
                    _paramLinks.Add(property, -1);
                }
            }
        }

        public virtual void AppyGlobal()
        {

        }

        //todo: update only changed params
        public virtual void Use()
        {
            GL.UseProgram(Handle);

            foreach (var link in _paramLinks)
            {
                var value = link.Key.GetValue(this);

                switch (value)
                {
                    case Matrix4 mtx4:
                        GL.UniformMatrix4(link.Value, false, ref mtx4);
                        break;
                    case Matrix3 mtx3:
                        GL.UniformMatrix3(link.Value, false, ref mtx3);
                        break;
                    case Matrix2 mtx2:
                        GL.UniformMatrix2(link.Value, false, ref mtx2);
                        break;
                    case Vector2 vec2:
                        GL.Uniform2(link.Value, ref vec2);
                        break;
                    case Vector3 vec3:
                        GL.Uniform3(link.Value, ref vec3);
                        break;
                    case Vector4 vec4:
                        GL.Uniform4(link.Value, ref vec4);
                        break;
                    case Color4 color4:
                        GL.Uniform4(link.Value, color4);
                        break;
                    case float num:
                        GL.Uniform1(link.Value, num);
                        break;
                    case int num:
                        GL.Uniform1(link.Value, num);
                        break;
                    case bool boolean:
                        GL.Uniform1(link.Value, boolean ? 1 : 0);
                        break;
                    case Texture texture:
                        GL.BindTexture(TextureTarget.Texture2D, texture.Handle);
                        break;
                    case null:
                        GL.Uniform1(link.Value, -1);
                        break;
                    default:
                        throw new NotSupportedException($"Type {value.GetType()} is not supported in shader programm");
                }
            }
        }
    }
}