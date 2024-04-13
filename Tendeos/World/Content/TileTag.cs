using Microsoft.Xna.Framework.Graphics;
using System;
using Tendeos.Inventory;
using Tendeos.Physical;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;

namespace Tendeos.World.Content
{
    public class TileTag : ITile
    {
        public float Health => -1;
        public byte Hardness => 0;
        public string Tag => null;
        public string Folder { get => null; set { } }
        public string Name => null;
        public string Description => null;
        public int MaxCount => -1;
        public Sprite ItemSprite => default;
        public bool Flip => false;
        public bool Animated => false;
        public bool Wallable => false;
        public bool ShadowAvailable => false;
        public float ShadowIntensity => 0;
        public IItem Drop => null;
        public Range DropCount => 0..0;
        public int DataCount => 0;
        public bool Collision => false;


        public void Changed(bool top, IMap map, int x, int y, TileData data) { }

        public void Destroy(bool top, IMap map, int x, int y, TileData data) { }

        public void Draw(SpriteBatch spriteBatch, bool top, IMap map, int x, int y, Vec2 drawPosition, TileData data) { }

        public void Loaded(bool top, IMap map, int x, int y, TileData data) { }

        public void Start(bool top, IMap map, int x, int y, TileData data) { }

        public void Update(IMap map, int x, int y, TileData data) { }

        public void Use(IMap map, ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData) { }

        public void With(SpriteBatch spriteBatch, IMap map, ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData) { }
    }
}
