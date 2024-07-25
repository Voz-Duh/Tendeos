using Tendeos.Content.Utlis;
using Tendeos.Physical;
using Tendeos.Physical.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;

namespace Tendeos.World.Content
{
    public class EnterableTile : Multitile
    {
        [SpriteLoad("@_open")] public Sprite spriteOpen;
        public Vec2 OpenDrawOffset { get; set; }
        public bool Flipable;

        public EnterableTile()
        {
            Collision = true;
        }

        public override void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition,
            TileData data)
        {
            if (data.HasCollision)
                base.Draw(spriteBatch, top, map, x, y, drawPosition, data);
            else
                spriteBatch.Rect(spriteOpen, drawPosition + (
                    Flipable && data.GetBool(0)
                        ? new Vec2(-OpenDrawOffset.X, OpenDrawOffset.Y)
                        : OpenDrawOffset), flipX: data.GetBool(0));
        }

        public override void Use(IMap map, ref TileData data, Player player)
        {
            data.HasCollision = !data.HasCollision;
            map.TryUnuseTile();
        }

        public override void OnPlace(IMap map, ITransform transform, ref TileData data)
        {
            if (Flipable) data.SetBool(0, transform.World2Local(Mouse.Position).X < 0);
        }
    }
}