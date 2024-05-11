using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Effect = Tendeos.Utils.Effect;

namespace Tendeos.World
{
    public static class EffectManager
    {
        public static List<Effect> Effects { get; } = new List<Effect>();

        public static void Add(Effect effect)
        {
            Effects.Add(effect);
        }
        public static void Remove(Effect effect)
        {
            Effects.Remove(effect);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Effect effect in Effects.ToArray())
            {
                effect.Draw(spriteBatch);
            }
        }

        public static void Update()
        {
            foreach (Effect effect in Effects.ToArray())
            {
                effect.Update();
            }
        }

        public static void Clear() => Effects.Clear();
    }
}
