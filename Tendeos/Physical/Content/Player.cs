using System;
using Microsoft.Xna.Framework;
using Tendeos.Inventory;
using Tendeos.Inventory.Content;
using Tendeos.Utils;
using Tendeos.Utils.Graphics;
using Tendeos.Utils.Input;
using Tendeos.Utils.SaveSystem;
using Tendeos.World;

namespace Tendeos.Physical.Content
{
    public class Player : SpawnEntity, ITransform
    {
        public const int ArmStates = 5, ArmStateLines = 2, UseRange = 4;
        public const float Width = 9, Height = 19, BaseSpeed = 30, JumpPower = 75;
        
        public const float MaxHealth = 100;

        public static readonly Color[] SkinColors = {
            new HEXColor(0xffe9deff),
            new HEXColor(0xe7c8b8ff),
            new HEXColor(0xdfb6a2ff),
            new HEXColor(0xd7a58aff),
            new HEXColor(0xc88e73ff),
            new HEXColor(0xaf8174ff),
            new HEXColor(0xa5725bff),
            new HEXColor(0x8f6053ff),
            new HEXColor(0x6b3e26ff),
            new HEXColor(0x683b32ff),
            new HEXColor(0x523529ff),
            new HEXColor(0x402824ff)
        };

        public readonly BodyTransform transform;
        public readonly Inventory.Inventory inventory;

        private readonly IMap map;
        
        private readonly PlayerInfo info;
        
        private readonly Sprite headSprite;
        private readonly Sprite[] armLSprites;
        private readonly Sprite[] armRSprites;
        private readonly Sprite[] bodySprites;
        private readonly Sprite[] legsSprites;
        private readonly Sprite[] legsMoveSprites;

        private readonly ArmData armData;
        
        public int XMovement;
        public bool MouseOnGUI, LeftDown, RightDown;
        public Vec2 LookDirection;
        
        public float Health { get; private set; }
        
        private Vec2 offset;

        private byte inArm;

        private float armLRotation, armRRotation;
        private byte armsState;

        private bool flip;
        private bool onFloor;
        private bool moving;

        private float legsAnimationTimer;
        private float armsAnimationTimer;
        private bool hidden;
        private Vec2 hitVelocity;

        public override Vec2 Position => transform.Position;

        public Player(Inventory.Inventory inventory, IMap map, Assets assets, PlayerInfo info)
        {
            Collider collider = Physics.Create(Width, Height, 1, 0);
            collider.tag = this;

            transform = new BodyTransform(collider);
            this.inventory = inventory;
            this.map = map;
            this.info = info;
            headSprite = assets.GetSprite($"player/{(info.Sex ? 'w' : 'm')}_head");
            armLSprites = assets.GetSprite($"player/{(info.Sex ? 'w' : 'm')}_arm_l").Split(ArmStates, ArmStateLines, 1);
            armRSprites = assets.GetSprite($"player/{(info.Sex ? 'w' : 'm')}_arm_r").Split(ArmStates, ArmStateLines, 1);
            bodySprites = assets.GetSprite($"player/{(info.Sex ? 'w' : 'm')}_body_{info.BodyType}").Split(4, 1, 1);
            Sprite[] sprites = assets.GetSprite($"player/{(info.Sex ? 'w' : 'm')}_legs").Split(8, 1, 1);
            legsMoveSprites = sprites[2..8];
            legsSprites = sprites[0..2];
            armData = new ArmData();

            Health = MaxHealth;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (hidden) return;

            offset = Vec2.Zero;
            int bodySprite = 0;
            int legSprite;
            if (onFloor)
            {
                if (moving)
                {
                    int anim = legsMoveSprites.Animation(SpriteHelper.frameRate, ref legsAnimationTimer,
                        flip != transform.flipX);

                    if (flip != transform.flipX)
                    {
                        switch (anim)
                        {
                            case 0:
                                offset = new Vec2(0, 1);
                                bodySprite = 0;
                                break;
                            case 1:
                                offset = new Vec2(0, 0);
                                bodySprite = 0;
                                break;
                            case 2:
                                offset = new Vec2(0, 0);
                                bodySprite = 3;
                                break;
                            case 3:
                                offset = new Vec2(0, 1);
                                bodySprite = 0;
                                break;
                            case 4:
                                offset = new Vec2(0, 0);
                                bodySprite = 0;
                                break;
                            case 5:
                                offset = new Vec2(0, 0);
                                bodySprite = 3;
                                break;
                        }
                    }
                    else
                    {
                        switch (anim)
                        {
                            case 0:
                                offset = new Vec2(1, 1);
                                bodySprite = 1;
                                break;
                            case 1:
                                offset = new Vec2(1, 0);
                                bodySprite = 1;
                                break;
                            case 2:
                                offset = new Vec2(1, 0);
                                bodySprite = 2;
                                break;
                            case 3:
                                offset = new Vec2(1, 1);
                                bodySprite = 1;
                                break;
                            case 4:
                                offset = new Vec2(1, 0);
                                bodySprite = 1;
                                break;
                            case 5:
                                offset = new Vec2(1, 0);
                                bodySprite = 2;
                                break;
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

            // set drawing flip x to player flip
            spriteBatch.FlipX = transform.flipX;
            int playerInventoryEnd = Core.PlayerInventoryStyle.Width * Core.PlayerInventoryStyle.Height;

            Helmet helmet = inventory.Items[playerInventoryEnd].item as Helmet;
            Cuirass cuirass = inventory.Items[playerInventoryEnd + 1].item as Cuirass;
            Legging legging = inventory.Items[playerInventoryEnd + 2].item as Legging;
            int sex = info.Sex ? 0 : 1;
            Color skinColor = SkinColors[info.SkinColor];

            //// DRAW LEFT ARM
            Vec2 armLPosition = transform.Local2World(offset + new Vec2(2, -4));
            spriteBatch.Rect(skinColor, armLSprites[armsState], armLPosition, 1, armLRotation);

            if (cuirass != null)
            {
                spriteBatch.Rect(cuirass.LeftArmSprite[sex][armsState], armLPosition, 1, armLRotation);
            }

            //// DRAW LEGS in idle and moving states
            Vec2 legsPosition = transform.Position + Vec2.UnitY * 3;
            if (onFloor)
            {
                spriteBatch.Rect(skinColor, moving ? legsMoveSprites[legSprite] : legsSprites[0], legsPosition);

                if (legging != null)
                {
                    spriteBatch.Rect(moving ? legging.MoveSprites[sex][legSprite] : legging.IdleSprites[sex][legSprite],
                        legsPosition);
                }
            }

            //// DRAW BODY
            Vec2 bodyPosition = transform.Local2World(new Vec2(0, offset.Y));
            spriteBatch.Rect(skinColor, bodySprites[bodySprite], bodyPosition);

            if (cuirass != null)
            {
                spriteBatch.Rect(cuirass.BodySprite[sex][info.BodyType][bodySprite], bodyPosition);
            }

            //// DRAW HEAD
            Vec2 headPosition = transform.Local2World(offset - Vec2.UnitY * 5);
            spriteBatch.Rect(skinColor, headSprite, headPosition, 1, 0, 0.5f, 1);

            if (helmet != null)
            {
                spriteBatch.Rect(helmet.Sprite[sex], headPosition, 1, 0, 0.5f, 1);
            }

            //// DRAW LEGS in air state
            if (!onFloor)
            {
                spriteBatch.Rect(skinColor, legsSprites[1], legsPosition);

                if (legging != null)
                {
                    spriteBatch.Rect(legging.IdleSprites[sex][1], legsPosition);
                }
            }

            //// DRAW ITEM
            // get item in arm
            IItem item = inventory.Items[inArm].item;
            // flip item draw if is needed
            if (item != null)
            {
                // don't flip drawing in x if is needed
                if (!item.Flip) spriteBatch.FlipX = false;
                // draw item in arm
                item.InArmDraw(
                    spriteBatch,
                    map, this,
                    armsState,
                    armLRotation,
                    armRRotation,
                    armData);
                // flip drawing back
                spriteBatch.FlipX = transform.flipX;
            }

            //// DRAW RIGHT ARM
            Vec2 armRPosition = transform.Local2World(offset + new Vec2(-2, -4));
            spriteBatch.Rect(skinColor, armRSprites[armsState], armRPosition, 1, armRRotation);

            if (cuirass != null)
            {
                spriteBatch.Rect(cuirass.RightArmSprite[sex][armsState], armRPosition, 1, armRRotation);
            }

            // reset drawing flip x
            spriteBatch.FlipX = false;
        }

        public override void Update()
        {
            if (hidden) return;
            
            (IItem item, int count) = inventory.Items[inArm];
            armsState = 0;
            armLRotation = armRRotation = 0;

            onFloor = Physics.Overlap(new FRectangle(
                transform.Position.X - transform.body.halfSize.X + 0.1f,
                transform.Position.Y,
                transform.body.halfSize.X * 2 - 0.2f,
                transform.body.halfSize.Y + 0.1f));

            moving = XMovement != 0;
            flip = XMovement < 0;

            if (moving) TransformUtils.StepEffect(onFloor, XMovement, transform);

            Vec2 relativeMousePosition = LookDirection;

            if (item?.Flip ?? true)
            {
                if (moving) transform.flipX = flip;
            }
            else transform.flipX = relativeMousePosition.X > 0;

            transform.body.velocity.X = XMovement * BaseSpeed;

            if (!item?.Animated ?? true) armsAnimationTimer = 0;
            armData.Clear();
            item?.InArmUpdate(
                map, this,
                LookDirection,
                MouseOnGUI, LeftDown, RightDown,
                ref armsState,
                ref armLRotation,
                ref armRRotation,
                ref count,
                ref armsAnimationTimer,
                armData);
            if (count <= 0)
                inventory.Items[inArm] = default;
            else
                inventory.Items[inArm] = (item, count);

            if (Controls.ScrollRight)
            {
                if (inArm + 1 == 6) ChangeArm(0);
                else ChangeArm((byte) (inArm + 1));
            }

            if (Controls.ScrollLeft)
            {
                if (inArm - 1 == -1) ChangeArm(5);
                else ChangeArm((byte) (inArm - 1));
            }

            if (Keyboard.IsPressed(Keys.D1)) ChangeArm(0);
            if (Keyboard.IsPressed(Keys.D2)) ChangeArm(1);
            if (Keyboard.IsPressed(Keys.D3)) ChangeArm(2);
            if (Keyboard.IsPressed(Keys.D4)) ChangeArm(3);
            if (Keyboard.IsPressed(Keys.D5)) ChangeArm(4);
            if (Keyboard.IsPressed(Keys.D6)) ChangeArm(5);
            if (inventory.Items[inArm].item != null && Controls.Drop)
            {
                new Item(inventory.Items[inArm], transform.Position, true);
                inventory.Items[inArm] = default;
            }

            float useDistance = map.TileSize * UseRange;
            if (map.HasUsedTile && Vec2.Distance(map.UseTilePosition, transform.Position) > useDistance)
            {
                map.TryUnuseTile();
            }

            if (!Mouse.OnGUI && Controls.Use)
            {
                if (Vec2.Distance(map.Cell2World(map.World2Cell(Mouse.Position)), transform.Position) <= useDistance)
                {
                    map.UseTile(this, map.World2Cell(Mouse.Position));
                }
                else if (map.HasUsedTile)
                {
                    map.TryUnuseTile();
                }
            }
        }

        public void Hit(float damage, Vec2 position, float power)
        {
            hitVelocity += Vec2.Normalize(transform.Position - position) * power;

            Damage(damage);
        }
        
        public void Damage(float damage)
        {
            if ((Health -= damage) <= 0)
            {
                Dead();
            }
        }

        public void Dead()
        {
            hidden = true;
        }

        public void Jump()
        {
            if (onFloor) transform.body.velocity.Y = -JumpPower;
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

        public static byte GetState(int index, int line) => (byte) (index + line * ArmStates);

        [ToByte]
        public void ToByte(ByteBuffer buffer) => buffer
            // send transform info
            .Append(transform.Position.X)
            .Append(transform.Position.Y)
            .Append(transform.body.velocity.X)
            .Append(transform.body.velocity.Y)
            .Append(transform.flipX)
            // send control info
            .Append(inArm)
            .Append(XMovement)
            .Append(MouseOnGUI)
            .Append(LeftDown)
            .Append(RightDown)
            .Append(LookDirection.X)
            .Append(LookDirection.Y);

        [FromByte]
        public void FromByte(ByteBuffer buffer) => buffer
            // sync transform info
            .Read(out transform.body.position.X)
            .Read(out transform.body.position.Y)
            .Read(out transform.body.velocity.X)
            .Read(out transform.body.velocity.Y)
            .Read(out transform.flipX)
            // sync control info
            .Read(out inArm)
            .Read(out XMovement)
            .Read(out MouseOnGUI)
            .Read(out LeftDown)
            .Read(out RightDown)
            .Read(out LookDirection.X)
            .Read(out LookDirection.Y);

        public override byte[] NetworkSend()
        {
            // TODO: network send of player data
            throw new NotImplementedException();
        }

        public override void NetworkAccept(byte[] data)
        {
            // TODO: network accept of player data
            throw new NotImplementedException();
        }
    }
}