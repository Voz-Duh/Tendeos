using XnaGame.PEntities;
using XnaGame.Utils.Graphics;

namespace XnaGame.Utils
{
    public class Effect : Entity
    {
        public FVector2 position;
        public float rotation;
        private FVector2
            emitLivetime = FVector2.One,
            emitRotation = FVector2.Zero,
            emitSize = FVector2.One,
            emitSpeed = FVector2.Zero,
            emits = FVector2.One;
        private Sprite[] emitAnimation;
        private float frameRate;
        private Particle[] particles;

        private float maxTime;
        private float timer;

        public Effect()
        {
        }

        private Effect(FVector2 position, float rotation, FVector2 emitLivetime, FVector2 emitRotation, FVector2 emitSize, FVector2 emitSpeed, FVector2 emits, Sprite[] emitAnimation, float frameRate, Particle[] particles, float maxTime)
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

        public Effect Spawn(FVector2 position, float angle)
        {
            int len = URandom.Int((int)emits.X, (int)emits.Y);
            float maxTime = 0;
            Particle[] particles = new Particle[len];
            for (int i = 0; i < len; i++)
            {
                float time = URandom.Float(emitLivetime.X, emitLivetime.Y);
                particles[i] = new Particle
                {
                    livetime = time,
                    rotation = URandom.Float(emitRotation.X, emitRotation.Y),
                    size = URandom.Float(emitSize.X, emitSize.Y),
                    speed = URandom.Float(emitSpeed.X, emitSpeed.Y),
                    position = position
                };
                if (maxTime < time) maxTime = time;
            }
            Effect clone = new Effect(position, angle, emitLivetime, emitRotation, emitSize, emitSpeed, emits, emitAnimation, frameRate, particles, maxTime);
            Core.AddEntity(clone.Draw, clone.Update);
            return clone;
        }

        public override void Draw()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                Particle p = particles[i];
                if (timer > p.livetime) continue;
                SDraw.Rect(emitAnimation[emitAnimation.Animation(frameRate, timer)], p.position + FVector2.RightOf(p.rotation + rotation) * timer * p.speed, p.rotation + rotation, p.size);
            }
        }

        public override void Update()
        {
            if (timer > maxTime) Remove();
            timer += Time.Delta;
        }

        #region Sets
        public void SetLivetime(float min, float max) => emitLivetime = new FVector2(min, max);
        public void SetLivetime(float value) => emitLivetime = new FVector2(value);

        public void SetRotation(float min, float max) => emitRotation = new FVector2(min, max);
        public void SetRotation(float value) => emitRotation = new FVector2(value);

        public void SetSize(float min, float max) => emitSize = new FVector2(min, max);
        public void SetSize(float value) => emitSize = new FVector2(value);

        public void SetSpeed(float min, float max) => emitSpeed = new FVector2(min, max);
        public void SetSpeed(float value) => emitSpeed = new FVector2(value);

        public void SetEmits(int min, int max) => emits = new FVector2(min, max);
        public void SetEmits(int value) => emits = new FVector2(value);

        public void SetDraw(Sprite sprite) => emitAnimation = new Sprite[] { sprite };
        public void SetDraw(Sprite[] animation, float frameRate, bool livetime = false)
        {
            emitAnimation = animation;
            this.frameRate = frameRate;
            if (livetime)
            {
                emitLivetime = new FVector2((animation.Length-1) * frameRate);
            }
        }
        #endregion

        private struct Particle
        {
            public float livetime, rotation, size, speed;
            public FVector2 position;
        }
    }
}
