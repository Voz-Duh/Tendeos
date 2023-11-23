using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGame.Inventory;
using XnaGame.Utils;

namespace XnaGame.WorldMap
{
    public interface ITile : IItem
    {
        byte Health { get; }
        byte Hardness { get; }
        void Changed(IMap map, int x, int y, TileData data);
        void Update(IMap map, int x, int y, TileData data);
        void Start(IMap map, int x, int y, TileData data);
        void Draw(IMap map, int x, int y, FVector2 drawPosition, TileData data);
        byte[] GetData();
    }
}
