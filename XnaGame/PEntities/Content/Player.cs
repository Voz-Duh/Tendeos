using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using XnaGame.Inventory;
using XnaGame.UI;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;
using XnaGame.WorldMap;

namespace XnaGame.PEntities.Content
{
    public class Player : SpawnEntity, ITransform
    {
        public const int armStates = 5, armStateLines = 2;
        public const float width = 9, height = 19, baseSpeed = 30, jumpPower = 60;
        
        public readonly BodyTransform transform;
        public readonly InventoryContainer inventory;

        private readonly Sprite headSprite;
        private readonly Sprite[] armLSprites;
        private readonly Sprite[] armRSprites;
        private readonly Sprite[] bodySprites;
        private readonly Sprite[] legsSprites;
        private readonly Sprite[] legsMoveSprites;

        private readonly ArmData armData;

        private FVector2 offset;

        private byte inArm;

        private float armLRotation, armRRotation;
        private byte armsState;

        private bool onFloor;
        private bool moving;

        private float legsAnimationTimer;
        private float armsAnimationTimer;

        public Player(GUIElement GUI, Sprite headSprite, Sprite armLSprite, Sprite armRSprite, Sprite bodySprite, Sprite legsSprite) : base()
        {
            Body body = new Body();
            Fixture fixture = body.CreateSmoothRectangle(1, width, height, 1, FVector2.Zero);
            fixture.Friction = 0;
            fixture.CollisionCategories = Category.Cat2;
            fixture.CollidesWith = Category.Cat1;

            body.BodyType = BodyType.Dynamic;
            body.FixedRotation = true;
            body.Tag = this;
            body.SleepingAllowed = false;
            Core.world.Add(body);
            transform = new BodyTransform(body);
            this.headSprite = headSprite;
            armLSprites = armLSprite.Split(armStates, armStateLines, 1);
            armRSprites = armRSprite.Split(armStates, armStateLines, 1);
            bodySprites = bodySprite.Split(3, 1, 1);
            Sprite[] sprites = legsSprite.Split(8, 1, 1);
            legsMoveSprites = sprites[2..8];
            legsSprites = sprites[0..2];
            inventory = new InventoryContainer(GUI, transform);
            armData = new ArmData();
            
            Core.player = this;
        }

        public override void Draw()
        {
            offset = FVector2.Zero;
            int bodySprite = 0;
            int legSprite;
            if (onFloor)
            {
                if (moving)
                {
                    int anim = legsMoveSprites.Animation(SpriteHelpers.frameRate, ref legsAnimationTimer);

                    switch (anim)
                    {
                        case 0: offset = new FVector2(1, 1); bodySprite = 1; break;
                        case 1: offset = new FVector2(1, 0); bodySprite = 1; break;
                        case 2: offset = new FVector2(1, 0); bodySprite = 2; break;
                        case 3: offset = new FVector2(1, 1); bodySprite = 1; break;
                        case 4: offset = new FVector2(1, 0); bodySprite = 1; break;
                        case 5: offset = new FVector2(1, 0); bodySprite = 2; break;
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

            SDraw.SpriteEffects = transform.flipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            SDraw.Rect(armLSprites[armsState], transform.Local2World(offset + new FVector2(2, -4)), armLRotation, 1, 0);
            if (onFloor && moving)
                SDraw.Rect(legsMoveSprites[legSprite], transform.Position + FVector2.UnitY * 3, 0, 1, 0);
            else if (onFloor)
                SDraw.Rect(legsSprites[0], transform.Position + FVector2.UnitY * 3, 0, 1, 0);
            SDraw.Rect(bodySprites[bodySprite], transform.Local2World(new FVector2(0, offset.Y)), 0, 1, 0);
            SDraw.Rect(headSprite, transform.Local2World(offset - FVector2.UnitY * 5), 0, 1, 0, Origin.Center, Origin.One);
            if (!onFloor) SDraw.Rect(legsSprites[1], transform.Position + FVector2.UnitY * 3, 0, 1, 0);
            IItem item = inventory.items[0, inArm].item;
            if (!item?.Flip ?? false) SDraw.SpriteEffects = SpriteEffects.None;
            item?.With(this, armsState, armLRotation, armRRotation, armData);
            if (!item?.Flip ?? false) SDraw.SpriteEffects = transform.flipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SDraw.Rect(armRSprites[armsState], transform.Local2World(offset + new FVector2(-2, -4)), armRRotation, 1, 0);

            SDraw.SpriteEffects = SpriteEffects.None;
        }

        public override void Update()
        {
            (IItem item, int count) = inventory.items[0, inArm];
            armsState = 0;
            armLRotation = armRRotation = 0;

            onFloor = false;
            FVector2 floorVelocity = FVector2.Zero;
            RayCastReportFixtureDelegate rayCast = (fixture, point, normal, fraction) =>
            {
                if (fixture.Body.Tag is IMap)
                {
                    onFloor = true;
                    floorVelocity = fixture.Body.GetLinearVelocityFromWorldPoint(point);
                    return 0;
                }
                return -1;
            };
            Core.world.RayCast(rayCast,
                transform.Position,
                transform.Position + FVector2.UnitY * (height / 2 + 0.1f));
            Core.world.RayCast(rayCast,
                transform.Position + new FVector2(width / 2 - 0.1f, 0),
                transform.Position + new FVector2(width / 2 - 0.1f, height / 2 + 0.1f));
            Core.world.RayCast(rayCast,
                transform.Position + new FVector2(-width / 2 + 0.1f, 0),
                transform.Position + new FVector2(-width / 2 + 0.1f, height / 2f + 0.1f));

            float yVel = transform.body.LinearVelocity.Y;

            int x = 0;
            if (Keyboard.IsDown(Keys.D)) x++;
            if (Keyboard.IsDown(Keys.A)) x--;
            moving = x != 0;
            if (item?.Flip ?? true)
            {
                if (moving)
                    transform.flipX = x < 0;
            }
            else
                transform.flipX = Mouse.Position.X - transform.Position.X < 0;

            if (onFloor && Keyboard.IsDown(Keys.Space)) yVel = -jumpPower;

            transform.body.LinearVelocity = new FVector2(x * baseSpeed, yVel) + floorVelocity;

            Core.camera.Position = transform.Position;


            if (!item?.Animated ?? true) armsAnimationTimer = 0;
            armData.Clear();
            item?.Use(this, ref armsState, ref armLRotation, ref armRRotation, ref count, ref armsAnimationTimer, armData);
            if (count <= 0)
                inventory.items[0, inArm] = default;
            else
                inventory.items[0, inArm] = (item, count);

            if (Keyboard.IsPressed(Keys.D1)) ChangeArm(0);
            if (Keyboard.IsPressed(Keys.D2)) ChangeArm(1);
            if (Keyboard.IsPressed(Keys.D3)) ChangeArm(2);
            if (Keyboard.IsPressed(Keys.D4)) ChangeArm(3);
            if (Keyboard.IsPressed(Keys.D5)) ChangeArm(4);
            if (Keyboard.IsPressed(Keys.Q))
            {
                if (inventory.items[0, inArm].item == null) goto SKIP;
                new Item(inventory.items[0, inArm], transform.Position);
                inventory.items[0, inArm] = default;
                SKIP:;
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

        public FVector2 Local2World(FVector2 point) => transform.Local2World(point + offset);
        public FVector2 World2Local(FVector2 point) => transform.World2Local(point - offset);

        public static byte GetState(int index, int line) => (byte)(index + line * armStates);
    }
}
