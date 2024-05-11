using Microsoft.Xna.Framework.Graphics;
using Tendeos.Utils;
using Tendeos.World;
using Tendeos.World.Content;

namespace Tendeos.Modding.Content
{
    public class ModTile : Tile, IModTile
    {
        public IModScript script { get; }
        public IModMethod draw, update, start, destroy, changed, loaded;

        public ModTile(IModScript script)
        {
            this.script = script;
            if (script?.has("draw") ?? false) draw = script.function("draw");
            if (script?.has("start") ?? false) start = script.function("start");
            if (script?.has("loaded") ?? false) loaded = script.function("loaded");
            if (script?.has("changed") ?? false) changed = script.function("changed");
            if (script?.has("destroy") ?? false) destroy = script.function("destroy");
        }

        public override void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data)
        {
            ModTileData tempData = new ModTileData(data);
            draw?.call(tempData, top, map, x, y, drawPosition);
        }

        public override void Changed(bool top, IMap map, int x, int y, ref TileData data)
        {
            ModTileData tempData = new ModTileData(data);
            changed?.call(tempData, top, map, x, y);
            data = tempData.tileData;
        }

        public override void Start(bool top, IMap map, int x, int y, ref TileData data)
        {
            ModTileData tempData = new ModTileData(data);
            start?.call(tempData, top, map, x, y);
            data = tempData.tileData;
        }

        public override void Destroy(bool top, IMap map, int x, int y, TileData data)
        {
            ModTileData tempData = new ModTileData(data);
            destroy?.call(tempData, top, map, x, y);
        }
        
        public override void Loaded(bool top, IMap map, int x, int y, ref TileData data)
        {
            ModTileData tempData = new ModTileData(data);
            loaded?.call(tempData, top, map, x, y);
            data = tempData.tileData;
        }
    }
}
