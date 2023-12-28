using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Reflection;
using XnaGame.WorldMap.Liquid;

namespace XnaGame.Content
{
    public static class Liquids
    {
        public static Liquid water, foo;

        public static void Init(ContentManager content)
        {
            water = new Liquid(Color.Aqua);
            foo = new Liquid(Color.BlueViolet);
        }

        public static Func<T> Get<T>(string value) where T : Liquid
        {
            FieldInfo t = typeof(Liquids).GetField(value);
            return () =>
            {
                if (cash.TryGetValue(value, out Liquid entity))
                    return (T)entity;
                T res = (T)t.GetValue(null);
                cash.Add(value, res);
                return res;
            };
        }

        private static Dictionary<string, Liquid> cash = new Dictionary<string, Liquid>();
    }
}
