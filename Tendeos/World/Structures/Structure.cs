using System;
using Tendeos.Modding;
using Tendeos.Content;

namespace Tendeos.World.Structures
{
    public class Structure
    {
        private (ITile w, ITile t)[][] data;

        private int width, height;
        public int Width => width;
        public int Height => height;

        public Structure(string code)
        {
            MISObject obj = MIS.GenerateVirtual(code, "tendeos.structure");
            obj.Require("struct", (MISArray array) =>
            {
                height = array.Length;
                int i, j;
                for (i = 0; i < array.Length; i++)
                    width = Math.Max(Width, array.Get<MISArray>(i).Length);
                data = new (ITile, ITile)[array.Length][];
                for (i = 0; i < array.Length; i++)
                {
                    MISArray row = array.Get<MISArray>(i);
                    data[i] = new (ITile, ITile)[row.Length];
                    for (j = 0; j < row.Length; j++)
                    {
                        MISArray obj = row.Get<MISArray>(j);
                        data[i][j].w = Tiles.Get(obj.Get<MISKey>(0).value);
                        data[i][j].t = Tiles.Get(obj.Get<MISKey>(1).value);
                    }
                }
            });
        }

        public void Spawn(IMap map, int x, int y)
        {
            for (int i = 0; i < data.Length; i++)
                for (int j = 0; j < data[i].Length; j++)
                {
                    map.SetTile(false, data[i][j].w, x + j, y + i);
                    map.SetTile(true, data[i][j].t, x + j, y + i);
                }
        }
    }
}
