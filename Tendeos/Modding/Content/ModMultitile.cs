using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.World;
using Tendeos.World.Content;

namespace Tendeos.Modding.Content
{
    public class ModMultitile : Multitile, IModTile
    {
        public IModScript script { get; }
        private IModMethod draw, update, start, destroy, changed, loaded;

        public ModMultitile(IModScript script)
        {
            this.script = script;
            if (script?.has("draw") ?? false) draw = script.function("draw");
            if (script?.has("start") ?? false) start = script.function("start");
            if (script?.has("loaded") ?? false) loaded = script.function("loaded");
            if (script?.has("changed") ?? false) changed = script.function("changed");
            if (script?.has("destroy") ?? false) destroy = script.function("destroy");
        }


        public override void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition,
            TileData data)
        {
            if (draw == null) base.Draw(spriteBatch, top, map, x, y, drawPosition, data);
            else
            {
                ModTileData tempData = new ModTileData(data);
                draw.call(tempData, top, map, x, y, drawPosition,
                    () => base.Draw(spriteBatch, top, map, x, y, drawPosition, data));
            }
        }

        public override void Changed(bool top, IMap map, int x, int y, ref TileData data)
        {
            if (changed == null) base.Changed(top, map, x, y, ref data);
            else
            {
                TileData tempData = data;
                changed.call(tempData, map, x, y, () => base.Changed(top, map, x, y, ref tempData));
                data = tempData;
            }
        }

        public override void Start(bool top, IMap map, int x, int y, ref TileData data)
        {
            if (start == null) base.Start(top, map, x, y, ref data);
            else
            {
                ModTileData tempData = new ModTileData(data);
                start.call(tempData, map, x, y, () => base.Start(top, map, x, y, ref tempData.tileData));
                data = tempData.tileData;
            }
        }

        public override void Destroy(bool top, IMap map, int x, int y, TileData data)
        {
            if (destroy == null) base.Destroy(top, map, x, y, data);
            else
            {
                ModTileData tempData = new ModTileData(data);
                destroy.call(tempData, top, map, x, y, () => base.Destroy(top, map, x, y, data));
            }
        }

        public override void Loaded(bool top, IMap map, int x, int y, ref TileData data)
        {
            if (destroy == null) base.Loaded(top, map, x, y, ref data);
            else
            {
                ModTileData tempData = new ModTileData(data);
                loaded.call(tempData, map, x, y, () => base.Loaded(top, map, x, y, ref tempData.tileData));
                data = tempData.tileData;
            }
        }
    }
}