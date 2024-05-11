using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Tendeos.Inventory;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;
using Tendeos.Utils.SaveSystem;
using Tendeos.Utils.SaveSystem.Content;
using Tendeos.World;

namespace Tendeos.Physical.Content
{
    public class Player : SpawnEntity, ITransform
    {
        public const int armStates = 5, armStateLines = 2;
        public const float width = 9, height = 19, baseSpeed = 30, jumpPower = 75;

        public readonly BodyTransform transform;
        public readonly Inventory.Inventory inventory;
        private readonly Camera camera;
        private readonly IMap map;
        private readonly PlayerInfo info;
        private readonly Sprite headSprite;
        private readonly Sprite[] armLSprites;
        private readonly Sprite[] armRSprites;
        private readonly Sprite[] bodySprites;
        private readonly Sprite[] legsSprites;
        private readonly Sprite[] legsMoveSprites;

        private readonly ArmData armData;

        private Vec2 offset;

        private byte inArm;

        private float armLRotation, armRRotation;
        private byte armsState;

        private bool flip;
        private bool onFloor;
        private bool moving;

        private float legsAnimationTimer;
        private float armsAnimationTimer;

        public Player(Inventory.Inventory inventory, Camera camera, IMap map, ContentManager content, PlayerInfo info)
        {
            Collider collider = Physics.Create(width, height, 1, 0);
            collider.tag = this;

            transform = new BodyTransform(collider);
            this.inventory = inventory;
            this.camera = camera;
            this.map = map;
            this.info = info;
            headSprite = Sprite.Load(content, $"player/{(info.sex ? 'w' : 'm')}_head");
            armLSprites = Sprite.Load(content, $"player/{(info.sex ? 'w' : 'm')}_arm_l").Split(armStates, armStateLines, 1);
            armRSprites = Sprite.Load(content, $"player/{(info.sex ? 'w' : 'm')}_arm_r").Split(armStates, armStateLines, 1);
            bodySprites = Sprite.Load(content, $"player/{(info.sex ? 'w' : 'm')}_body_{info.body}").Split(4, 1, 1);
            Sprite[] sprites = Sprite.Load(content, $"player/{(info.sex ? 'w' : 'm')}_legs").Split(8, 1, 1);
            legsMoveSprites = sprites[2..8];
            legsSprites = sprites[0..2];
            armData = new ArmData();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            offset = Vec2.Zero;
            int bodySprite = 0;
            int legSprite;
            if (onFloor)
            {
                if (moving)
                {
                    int anim = legsMoveSprites.Animation(SpriteHelper.frameRate, ref legsAnimationTimer, flip != transform.flipX);

                    if (flip != transform.flipX)
                    {
                        switch (anim)
                        {
                            case 0: offset = new Vec2(0, 1); bodySprite = 0; break;
                            case 1: offset = new Vec2(0, 0); bodySprite = 0; break;
                            case 2: offset = new Vec2(0, 0); bodySprite = 3; break;
                            case 3: offset = new Vec2(0, 1); bodySprite = 0; break;
                            case 4: offset = new Vec2(0, 0); bodySprite = 0; break;
                            case 5: offset = new Vec2(0, 0); bodySprite = 3; break;
                        }
                    }
                    else
                    {
                        switch (anim)
                        {
                            case 0: offset = new Vec2(1, 1); bodySprite = 1; break;
                            case 1: offset = new Vec2(1, 0); bodySprite = 1; break;
                            case 2: offset = new Vec2(1, 0); bodySprite = 2; break;
                            case 3: offset = new Vec2(1, 1); bodySprite = 1; break;
                            case 4: offset = new Vec2(1, 0); bodySprite = 1; break;
                            case 5: offset = new Vec2(1, 0); bodySprite = 2; break;
                        }
                    }

                    legSprite = anim;
                }
                else
                {
                    legsAnimationTimer = 0;
                    legSprite = 0;
                }
            }
            else
            {
                legsAnimationTimer = 0;
                legSprite = 1;
            }

            DrawSprite.SpriteEffects = transform.flipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Rect(armLSprites[armsState], transform.Local2World(offset + new Vec2(2, -4)), armLRotation, 1, 0);
            if (onFloor && moving)
                spriteBatch.Rect(legsMoveSprites[legSprite], transform.Position + Vec2.UnitY * 3, 0, 1, 0);
            else if (onFloor)
                spriteBatch.Rect(legsSprites[0], transform.Position + Vec2.UnitY * 3, 0, 1, 0);
            spriteBatch.Rect(bodySprites[bodySprite], transform.Local2World(new Vec2(0, offset.Y)), 0, 1, 0);
            spriteBatch.Rect(headSprite, transform.Local2World(offset - Vec2.UnitY * 5), 0, 1, 0, Origin.Center, Origin.One);
            if (!onFloor) spriteBatch.Rect(legsSprites[1], transform.Position + Vec2.UnitY * 3, 0, 1, 0);
            IItem item = inventory.Items[inArm].item;
            if (!item?.Flip ?? false) DrawSprite.SpriteEffects = SpriteEffects.None;
            item?.With(spriteBatch, map, this, armsState, armLRotation, armRRotation, armData);
            if (!item?.Flip ?? false) DrawSprite.SpriteEffects = transform.flipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Rect(armRSprites[armsState], transform.Local2World(offset + new Vec2(-2, -4)), armRRotation, 1, 0);

            DrawSprite.SpriteEffects = SpriteEffects.None;
        }

        public override void Update()
        {
            (IItem item, int count) = inventory.Items[inArm];
            armsState = 0;
            armLRotation = armRRotation = 0;

            onFloor = false;
            RaycastMapDelegate rayCast = (_, _, _, _, _) => onFloor = true;
            Physics.RaycastMap(rayCast,
                transform.Position + new Vec2(transform.body.halfSize.X - 0.1f, 0),
                new Vec2(0, transform.body.halfSize.Y + 2f));
            Physics.RaycastMap(rayCast,
                transform.Position + new Vec2(-transform.body.halfSize.X + 0.1f, 0),
                new Vec2(0, transform.body.halfSize.Y + 2f));

            float yVel = transform.body.velocity.Y;

            int x = 0;
            if (Keyboard.IsDown(Keys.D)) x++;
            if (Keyboard.IsDown(Keys.A)) x--;
            moving = x != 0;
            flip = x < 0;
            if (item?.Flip ?? true)
            {
                if (moving) transform.flipX = flip;
            }
            else
                transform.flipX = Mouse.Position.X - transform.Position.X < 0;

            if (onFloor && Keyboard.IsDown(Keys.Space)) yVel = -jumpPower;

            transform.body.velocity = new Vec2(x * baseSpeed, yVel);

            camera.Position = transform.Position;

            if (!item?.Animated ?? true) armsAnimationTimer = 0;
            armData.Clear();
            item?.Use(map, this, ref armsState, ref armLRotation, ref armRRotation, ref count, ref armsAnimationTimer, armData);
            if (count <= 0)
                inventory.Items[inArm] = default;
            else
                inventory.Items[inArm] = (item, count);

            if (Keyboard.IsPressed(Keys.D1)) ChangeArm(0);
            if (Keyboard.IsPressed(Keys.D2)) ChangeArm(1);
            if (Keyboard.IsPressed(Keys.D3)) ChangeArm(2);
            if (Keyboard.IsPressed(Keys.D4)) ChangeArm(3);
            if (Keyboard.IsPressed(Keys.D5)) ChangeArm(4);
            if (Keyboard.IsPressed(Keys.D6)) ChangeArm(5);
            if (inventory.Items[inArm].item != null && Keyboard.IsPressed(Keys.Q))
            {
                new Item(inventory.Items[inArm], transform.Position, true);
                inventory.Items[inArm] = default;
            }
        }

        public void ChangeArm(byte i)
        {
            if (inArm == i) return;

            armsAnimationTimer = 0;
            inArm = i;
        }

        public float Local2World(float degrees) => transform.Local2World(degrees);
        public float World2Local(float degrees) => transform.World2Local(degrees);

        public Vec2 Local2World(Vec2 point) => transform.Local2World(point + offset);
        public Vec2 World2Local(Vec2 point) => transform.World2Local(point - offset);

        public static byte GetState(int index, int line) => (byte)(index + line * armStates);

        [ToByte]
        public void ToByte(ByteBuffer buffer) => buffer
            .Append(transform.Position.X)
            .Append(transform.Position.Y)
            .Append(transform.body.velocity.X)
            .Append(transform.body.velocity.Y)
            .Append(transform.flipX)
            .Append(info)
            .Append(inArm)
            .Append(inventory);

        [FromByte]
        public void FromByte(ByteBuffer buffer) => buffer
            .Read(out transform.body.position.X).Read(out transform.body.position.Y)
            .Read(out transform.body.velocity.X).Read(out transform.body.velocity.Y)
            .Read(out transform.flipX)
            .Read(info)
            .Read(out inArm)
            .Read(inventory);

        public override byte[] NetworkSend()
        {
            throw new System.NotImplementedException();
        }

        public override void NetworkAccept(byte[] data)
        {
            throw new System.NotImplementedException();
        }
    }
}
