using XnaGame.Inventory;
using XnaGame.PEntities;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.World.Content
{
    public class TileTag : ITile
    {
        public float Health => -1;
        public byte Hardness => 0;
        public string Name => null;
        public string Description => null;
        public int MaxCount => -1;
        public Sprite ItemSprite => default;
        public bool Flip => false;
        public bool Animated => false;
        public bool Wallable => false;
        public bool ShadowAvailable => false;
        public float ShadowIntensity => 0;

        public void Changed(bool top, IMap map, int x, int y, TileData data) { }
        public void Draw(bool top, IMap map, int x, int y, Vec2 drawPosition, float angle, TileData data) { }

        public byte[] GetData() => null;

        public void Start(bool top, IMap map, int x, int y, TileData data) { }

        public void Update(IMap map, int x, int y, TileData data) { }

        public void Use(ITransform transform, ref byte armsState, ref float armLRotation, ref float armRRotation, ref int count, ref float timer, ArmData armData) { }

        public void With(ITransform transform, byte armsState, float armLRotation, float armRRotation, ArmData armData) { }
    }
}
