using System;
using GameCore.Render;

namespace GameCore.Entity
{
    public class BlockStaticData
    {
        public CubeTextureCoord TextureCoord { get; set; }
        public string Name { get; set; }
        public bool IsTransparent { get; set; }

        public BlockStaticData(string name, CubeTextureCoord textureCoord, bool isTransparent = false)
        {
            IsTransparent = isTransparent;
            TextureCoord = textureCoord;
            Name = name;
        }
    }

    public struct Block
    {
        private static readonly BlockStaticData[] BlockStaticDatas = new BlockStaticData[256];

        public byte Id { get; set; }
        public byte Data { get; set; }
        public BlockStaticData StaticData => BlockStaticDatas[Id];

        public static Block FindByName(string name)
        {
            name = name.ToLower();

            for (byte i = 0; i < BlockStaticDatas.Length; i++)
            {
                if (BlockStaticDatas[i]?.Name == name)
                {
                    return new Block {Id = i};
                }
            }

            throw new Exception($"Block {name} is not registred");
        }

        public static void RegBlock(byte id, BlockStaticData data)
        {
            if (BlockStaticDatas[id] != null)
            {
                throw new Exception($"Block id {id} already taken {BlockStaticDatas[id].Name}");
            }

            data.Name = data.Name.ToLower();
            BlockStaticDatas[id] = data;
        }

        public static void RegStandartBlocks()
        {
            RegBlock(0, new BlockStaticData("void", new CubeTextureCoord()));
            RegBlock(1, new BlockStaticData("dirt", CubeTextureCoord.FromSolo(AtlasTextureCoord(2, 0))));
            RegBlock(2, new BlockStaticData("grass",
                CubeTextureCoord.FromT_B_LRFB(
                    AtlasTextureCoord(2, 9),
                    AtlasTextureCoord(2, 0),
                    AtlasTextureCoord(3, 0))
            ));

            RegBlock(3, new BlockStaticData("rock", CubeTextureCoord.FromSolo(AtlasTextureCoord(1, 0))));
            RegBlock(4, new BlockStaticData("adminium", CubeTextureCoord.FromSolo(AtlasTextureCoord(1, 1))));
        }

        private const int AtlasW = 16;
        private const int AtlasH = 16;

        private static TextureCoord AtlasTextureCoord(int x, int y)
        {
            return new TextureCoord(1f / AtlasW * x, 1f / AtlasH * y, 1f / AtlasW, 1f / AtlasW);
        }
    }
}