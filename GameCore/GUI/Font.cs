using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using GameCore.EMath;
using GameCore.Render;
using Newtonsoft.Json;

namespace GameCore.GUI
{
    public class FontDescription
    {
        public string FontName { get; set; }
        public string FontFile { get; set; }
        public int AtlasWidth { get; set; }
        public int AtlasHeight { get; set; }
        public int AtlasOffsetX { get; set; }
        public int AtlasOffsetY { get; set; }
        public int FontSize { get; set; }
        public bool DumpAtlasToFile { get; set; }
        public IEnumerable<Range> CharRanges { get; set; }
    }

    public struct FontCharInfo
    {
        public float AtlasW { get; set; }
        public float AtlasH { get; set; }
        public float AtlasX { get; set; }
        public float AtlasY { get; set; }

        public float SizeW { get; set; }
        public float SizeH { get; set; }
    }

    public class Font
    {
        private readonly Dictionary<char, FontCharInfo> _charMap = new Dictionary<char, FontCharInfo>();
        public Texture TextureAtlas { get; set; }
        public float BaseFontSize { get; set; }
        public string Name { get; set; }

        public Font(string fontPath)
        {
            var descFile = fontPath + ".json";
            if (!File.Exists(descFile))
            {
                Console.WriteLine($"Failed load font {fontPath}. Descriptor not found");
                return;
            }

            var desc = JsonConvert.DeserializeObject<FontDescription>(File.ReadAllText(descFile, Encoding.UTF8));
            BaseFontSize = desc.FontSize;
            Name = desc.FontName;

            using (var bitmap = new Bitmap(desc.AtlasWidth, desc.AtlasHeight, PixelFormat.Format32bppArgb))
            {
               var  font = GetFont(fontPath, desc);
                GenerateAtlas(bitmap, desc, font);

                if (desc.DumpAtlasToFile)
                    bitmap.Save(fontPath + ".png");

                TextureAtlas = new Texture("font." + fontPath, bitmap);
            }
        }

        private static System.Drawing.Font GetFont(string fontPath, FontDescription desc)
        {
            if (!string.IsNullOrEmpty(desc.FontFile))
            {
                var pathToFontFile = Path.Combine(Path.GetDirectoryName(fontPath), desc.FontFile);
                if (!File.Exists(pathToFontFile))
                {
                    Console.WriteLine($"Failed load font {desc.FontName}. File {pathToFontFile} not found");
                    return null;
                }

                var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), pathToFontFile);
                var collection = new PrivateFontCollection();
                collection.AddFontFile(absolutePath);
                var fontFamily = new FontFamily(Path.GetFileNameWithoutExtension(absolutePath), collection);
                return new System.Drawing.Font(fontFamily, desc.FontSize);
            }

            return new System.Drawing.Font(new FontFamily(desc.FontName), desc.FontSize);
        }

        private void GenerateAtlas(Image bitmap, FontDescription desc, System.Drawing.Font font)
        {
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                var x = desc.AtlasOffsetX;
                var y = desc.AtlasOffsetY;
                var maxY = 0;

                foreach (var range in desc.CharRanges)
                {
                    for (var c = (char) range.From; c < range.To; c++)
                    {
                        var s = c.ToString();
                        var size = g.MeasureString(s, font);

                        g.DrawString(s, font, Brushes.White, x, y);

                        _charMap.Add(c, new FontCharInfo
                        {
                            AtlasX = x / (float) desc.AtlasWidth,
                            AtlasY = y / (float) desc.AtlasHeight,
                            AtlasW = size.Width / desc.AtlasWidth,
                            AtlasH = size.Height / desc.AtlasHeight,
                            SizeW = size.Width,
                            SizeH = size.Height,
                        });

                        x += (int) size.Width + desc.AtlasOffsetX;
                        maxY = Math.Max(maxY, (int) size.Height);

                        if (x >= desc.AtlasWidth)
                        {
                            y += maxY + desc.AtlasOffsetY;
                            x = desc.AtlasOffsetX;
                            maxY = 0;
                        }
                    }
                }
            }

            _charMap[' '] = new FontCharInfo
            {
                 SizeW = BaseFontSize
            };
        }

        public IEnumerable<FontCharInfo> MapString(string text)
        {
            return text.Select(x => _charMap.GetValueOrDefault(x));
        }
    }
}