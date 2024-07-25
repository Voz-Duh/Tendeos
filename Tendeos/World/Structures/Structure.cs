using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Tendeos.Modding;
using Tendeos.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.World.Structures
{
    public class Structure
    {
        private (ITile w, ITile t)[][] data;

        private int width, height;
        public int Width => width;
        public int Height => height;

        public Structure(MISObject obj)
        {
            if (obj.Check("struct", (string path) =>
                {
                    Dictionary<Color, (string, string)> colorIndicators = new();
                    obj.Require("colors", (MISObject obj) =>
                    {
                        foreach (var (key, tile, wall) in obj.GetAllParametersAs<string, string>())
                        {
                            if (key[0] != 'x')
                                throw new InvalidCastException($"Parameter {key} is not a color HEX.");

                            string hex = key.Substring(1);
                            foreach (char ch in hex)
                            {
                                if (!Core.HEXNumbers.Contains(ch))
                                    throw new InvalidCastException($"Parameter {key} is not a color HEX.");
                            }

                            colorIndicators[new HEXColor(Convert.ToUInt32(hex, 16))] = (tile, wall);
                        }
                    });

                    Sprite sprite = Core.Game.Assets.GetSprite(path);
                    Color[] spriteData = sprite.Data;
                    data = new (ITile, ITile)[sprite.Rect.Height][];
                    for (int i = 0; i < sprite.Rect.Height; i++)
                    {
                        data[i] = new (ITile, ITile)[sprite.Rect.Width];
                        for (int j = 0; j < sprite.Rect.Width; j++)
                        {
                            var (w, t) = colorIndicators[spriteData[i * sprite.Rect.Width + j]];
                            data[i][j] = (Tiles.Get(w), Tiles.Get(t));
                        }
                    }
                })) return;
            
            obj.Require("struct", (MISArray array) =>
            {
                height = array.Length;
                for (int i = 0; i < array.Length; i++)
                    width = Math.Max(Width, array.Get<MISArray>(i).Length);
                data = new (ITile, ITile)[array.Length][];
                for (int i = 0; i < array.Length; i++)
                {
                    MISArray row = array.Get<MISArray>(i);
                    data[i] = new (ITile, ITile)[row.Length];
                    for (int j = 0; j < row.Length; j++)
                    {
                        MISArray obj = row.Get<MISArray>(j);
                        data[i][j] = (Tiles.Get(obj.Get<MISKey>(0).value), Tiles.Get(obj.Get<MISKey>(1).value));
                    }
                }
            });
        }

        public void Spawn(IMap map, int x, int y)
        {
            int j;
            for (int i = 0; i < data.Length; i++)
            for (j = 0; j < data[i].Length; j++)
            {
                map.DestroyTile(false, x + j, y + i);
                map.DestroyTile(true, x + j, y + i);
                map.SetTile(false, data[i][j].w, x + j, y + i);
                map.SetTile(true, data[i][j].t, x + j, y + i);
            }
        }
    }
}