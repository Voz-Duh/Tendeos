using Microsoft.Xna.Framework.Graphics;
using Tendeos.Physical;
using Tendeos.Utils.Graphics;
using Tendeos.World;

namespace Tendeos.Utils
{
    public class Effect : Entity
    {
        public Vec2 position;
        public float rotation;
        private Vec2
            emitLivetime = Vec2.One,
            emitRotation = Vec2.Zero,
            emitSize = Vec2.One,
            emitSpeed = Vec2.Zero,
            emits = Vec2.One;
        private Sprite[] emitAnimation;
        private float frameRate;
        private Particle[] particles;

        private float maxTime;
        private float timer;

        public Effect()
        {
        }

        private Effect(Vec2 position, float rotation, Vec2 emitLivetime, Vec2 emitRotation, Vec2 emitSize, Vec2 emitSpeed, Vec2 emits, Sprite[] emitAnimation, float frameRate, Particle[] particles, float maxTime)
        {
            this.position = position;
            this.rotation = rotation;
            this.emitLivetime = emitLivetime;
            this.emitRotation = emitRotation;
            this.emitSize = emitSize;
            this.emitSpeed = emitSpeed;
            this.emits = emits;
            this.emitAnimation = emitAnimation;
            this.frameRate = frameRate;
            this.particles = particles;
            this.maxTime = maxTime;
        }

        public Effect Spawn(Vec2 position, float angle)
        {
            int len = URandom.SInt((int)emits.X, (int)emits.Y);
            float maxTime = 0;
            Particle[] particles = new Particle[len];
            for (int i = 0; i < len; i++)
            {
                float time = URandom.SFloat(emitLivetime.X, emitLivetime.Y);
                particles[i] = new Particle
                {
                    livetime = time,
                    rotation = URandom.SFloat(emitRotation.X, emitRotation.Y),
                    size = URandom.SFloat(emitSize.X, emitSize.Y),
                    speed = URandom.SFloat(emitSpeed.X, emitSpeed.Y),
                    position = position
                };
                if (maxTime < time) maxTime = time;
            }
            Effect clone = new Effect(position, angle, emitLivetime, emitRotation, emitSize, emitSpeed, emits, emitAnimation, frameRate, particles, maxTime);
            EntityManager.Add(clone);
            return clone;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                Particle p = particles[i];
                if (timer > p.livetime) continue;
                spriteBatch.Rect(emitAnimation[emitAnimation.Animation(frameRate, timer)], p.position + Vec2.RightOf(p.rotation + rotation) * timer * p.speed, p.rotation + rotation, p.size);
            }
        }

        public override void Update()
        {
            if (timer > maxTime) Remove();
            timer += Time.Delta;
        }

        #region Sets
        public void SetLivetime(float min, float max) => emitLivetime = new Vec2(min, max);
        public void SetLivetime(float value) => emitLivetime = new Vec2(value);

        public void SetRotation(float min, float max) => emitRotation = new Vec2(min, max);
        public void SetRotation(float value) => emitRotation = new Vec2(value);

        public void SetSize(float min, float max) => emitSize = new Vec2(min, max);
        public void SetSize(float value) => emitSize = new Vec2(value);

        public void SetSpeed(float min, float max) => emitSpeed = new Vec2(min, max);
        public void SetSpeed(float value) => emitSpeed = new Vec2(value);

        public void SetEmits(int min, int max) => emits = new Vec2(min, max);
        public void SetEmits(int value) => emits = new Vec2(value);

        public void SetDraw(Sprite sprite) => emitAnimation = new Sprite[] { sprite };
        public void SetDraw(Sprite[] animation, float frameRate, bool livetime = false)
        {
            emitAnimation = animation;
            this.frameRate = frameRate;
            if (livetime)
            {
                emitLivetime = new Vec2((animation.Length - 1) * frameRate);
            }
        }
        #endregion

        private struct Particle
        {
            public float livetime, rotation, size, speed;
            public Vec2 position;
        }
    }
}
