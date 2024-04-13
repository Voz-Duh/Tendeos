﻿using Microsoft.Xna.Framework.Graphics;
using Tendeos.Utils;
using Tendeos.World;
using Tendeos.World.Content;

namespace Tendeos.Modding.Content
{
    public class ModAutoTile : AutoTile, IModTile
    {
        public IModScript script { get; }
        public IModMethod draw, update, start, destroy, changed, loaded;

        public ModAutoTile(IModScript script)
        {
            this.script = script;
            if (script?.has("draw") ?? false) draw = script.function("draw");
            if (script?.has("start") ?? false) start = script.function("start");
            if (script?.has("update") ?? false) update = script.function("update");
            if (script?.has("loaded") ?? false) loaded = script.function("loaded");
            if (script?.has("changed") ?? false) changed = script.function("changed");
            if (script?.has("destroy") ?? false) destroy = script.function("destroy");
        }

        public override void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data)
        {
            if (draw == null) base.Draw(spriteBatch, top, map, x, y, drawPosition, data);
            else draw.call(data, top, map, x, y, drawPosition, () => base.Draw(spriteBatch, top, map, x, y, drawPosition, data));
        }

        public override void Update(IMap map, int x, int y, TileData data)
        {
            if (update == null) base.Update(map, x, y, data);
            else update.call(data, map, x, y, () => base.Update(map, x, y, data));
        }

        public override void Changed(bool top, IMap map, int x, int y, TileData data)
        {
            if (changed == null) base.Changed(top, map, x, y, data);
            else changed.call(data, top, map, x, y, () => base.Changed(top, map, x, y, data));
        }

        public override void Start(bool top, IMap map, int x, int y, TileData data)
        {
            if (start == null) base.Start(top, map, x, y, data);
            else start.call(data, top, map, x, y, () => base.Start(top, map, x, y, data));
        }

        public override void Destroy(bool top, IMap map, int x, int y, TileData data)
        {
            if (destroy == null) base.Destroy(top, map, x, y, data);
            else destroy.call(data, top, map, x, y, () => base.Destroy(top, map, x, y, data));
        }

        public override void Loaded(bool top, IMap map, int x, int y, TileData data)
        {
            if (destroy == null) base.Loaded(top, map, x, y, data);
            else loaded.call(data, top, map, x, y, () => base.Loaded(top, map, x, y, data));
        }
    }
}