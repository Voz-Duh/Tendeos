﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Tendeos.World.Liquid;

namespace Tendeos.Content
{
    public static class Liquids
    {
        public static Liquid water, foo;

        public static void Init()
        {
            water = new Liquid(Color.Aqua);
            foo = new Liquid(Color.BlueViolet);
        }

        public static Liquid Get(string value) => (Liquid) typeof(Liquids).GetField(value).GetValue(null);
    }
}