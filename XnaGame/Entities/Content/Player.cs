using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using XnaGame.Inventory;
using XnaGame.UI;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;
using XnaGame.Utils.Input;
using XnaGame.WorldMap;

namespace XnaGame.Entities.Content
{
    public class Player : Entity
    {
        public const int armStates = 2;
        public const float width = 9, height = 19, baseSpeed = 30, jumpPower = 60;
        
        public readonly BodyTransform transform;
        public readonly InventoryContainer inventory;

        private readonly Sprite headSprite;
        private readonly Sprite[] armLSprites;
        private readonly Sprite[] armRSprites;
        private readonly Sprite[] bodySprites;
        private readonly Sprite[] legsSprites;
        private readonly Sprite[] legsMoveSprites;

        private float armsRotation;
        private byte inArm;
        private byte armsState;
        private bool onFloor;
        private bool moving;

        public Player(GUIElement GUI, Sprite headSprite, Sprite armLSprite, Sprite armRSprite, Sprite bodySprite, Sprite legsSprite)
        {
            Body body = new Body();
            body.CreateRectangle(width, height, 1, FVector2.Zero).Friction = 0;
            body.BodyType = BodyType.Dynamic;
            body.FixedRotation = true;
            body.Tag = this;
            Core.world.Add(body);
            transform = new BodyTransform(body);
            this.headSprite = headSprite;
            armLSprites = armLSprite.Split(armStates, 1, 1);
            armRSprites = armRSprite.Split(armStates, 1, 1);
            bodySprites = bodySprite.Split(3, 1, 1);
            Sprite[] sprites = legsSprite.Split(8, 1, 1);
            legsMoveSprites = sprites[2..8];
            legsSprites = sprites[0..2];
            inventory = new InventoryContainer(GUI, transform);
        }

        public override void Draw()
        {
            FVector2 offset = FVector2.Zero;
            int bodySprite = 0;
            int legSprite;
            if (onFloor)
            {
                if (moving)
                {
                    int anim = legsMoveSprites.Animation(.6f);

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
                else legSprite = 0;
            }
            else legSprite = 1;
            
            SDraw.SpriteEffects = transform.flipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            
            SDraw.Rect(armLSprites[armsState], transform.Local2World(offset), armsRotation, 1, 0);
            if (onFloor && moving)
                SDraw.Rect(legsMoveSprites[legSprite], transform.Position + FVector2.UnitY * 3, 0, 1, 0);
            else if (onFloor)
                SDraw.Rect(legsSprites[0], transform.Position + FVector2.UnitY * 3, 0, 1, 0);
            SDraw.Rect(bodySprites[bodySprite], transform.Local2World(new FVector2(0, offset.Y)), 0, 1, 0);
            SDraw.Rect(headSprite, transform.Local2World(offset - FVector2.UnitY * 5), 0, 1, 0, Origin.Center, Origin.One);
            if (!onFloor) SDraw.Rect(legsSprites[1], transform.Position + FVector2.UnitY * 3, 0, 1, 0);
            inventory.items[inArm, 0].item?.With(transform, ref armsState, ref armsRotation);
            SDraw.Rect(armRSprites[armsState], transform.Local2World(offset), armsRotation, 1, 0);

            SDraw.SpriteEffects = SpriteEffects.None;
        }

        public override void Update()
        {
            armsState = 0;
            armsRotation = 0;

            onFloor = false;
            Core.world.RayCast((fixture, point, normal, fraction) =>
                {
                    if (fixture.Body.Tag is IMap)
                    {
                        onFloor = true;
                        return 0;
                    }
                    return -1;
                },
                transform.Position + new FVector2(width / 2 - 0.1f, 0),
                transform.Position + new FVector2(width / 2 - 0.1f, height / 2 + 0.1f));
            Core.world.RayCast(
                (fixture, point, normal, fraction) =>
                {
                    if (fixture.Body.Tag is IMap)
                    {
                        onFloor = true;
                        return 0;
                    }
                    return -1;
                },
                transform.Position + new FVector2(-width / 2 + 0.1f, 0),
                transform.Position + new FVector2(-width / 2 + 0.1f, height / 2f + 0.1f));

            float yVel = transform.body.LinearVelocity.Y;

            int x = 0;
            if (Keyboard.IsDown(Keys.D)) x++;
            if (Keyboard.IsDown(Keys.A)) x--;
            if (moving = x != 0) transform.flipX = x < 0;

            if (onFloor && Keyboard.IsDown(Keys.Space)) yVel = -jumpPower;

            transform.body.LinearVelocity = new FVector2(x * baseSpeed, yVel);

            (IItem item, int count) = inventory.items[inArm, 0];
            item?.Use(transform, ref count);
            if (count <= 0)
                inventory.items[inArm, 0] = default;
            else
                inventory.items[inArm, 0] = (item, count);

            Core.camera.Position = transform.Position;

            if (Keyboard.IsPressed(Keys.D1)) inArm = 0;
            if (Keyboard.IsPressed(Keys.D2)) inArm = 1;
            if (Keyboard.IsPressed(Keys.D3)) inArm = 2;
            if (Keyboard.IsPressed(Keys.D4)) inArm = 3;
            if (Keyboard.IsPressed(Keys.D5)) inArm = 4;
            if (Keyboard.IsPressed(Keys.Q))
            {
                new Item(inventory.items[inArm, 0], transform.Position);
                inventory.items[inArm, 0] = (null, 0);
            }
        }
    }
}
