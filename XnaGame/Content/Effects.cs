using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Reflection;
using XnaGame.Utils;
using XnaGame.Utils.Graphics;

namespace XnaGame.Content
{
    public static class Effects
    {
        public static Effect slashMedium;

        public static void Init(ContentManager content)
        {
            //Sprite slashSmallSprite = Sprite.Load(content, "slash_small");

            slashMedium = new Effect();
            slashMedium.SetDraw(Sprite.Load(content, "effects/slash_medium").Split(3, 1, 1), 0.15f, true);
            slashMedium.SetEmits(1);
            slashMedium.SetSpeed(50);
        }

        public static Func<T> Get<T>(string value) where T : Effect
        {
            FieldInfo t = typeof(Effects).GetField(value);
            return () =>
            {
                if (cash.TryGetValue(value, out Effect entity))
                    return (T)entity;
                T res = (T)t.GetValue(null);
                cash.Add(value, res);
                return res;
            };
        }

        private static readonly Dictionary<string, Effect> cash = new Dictionary<string, Effect>();
    }

    public delegate Effect EffectRef();
}
