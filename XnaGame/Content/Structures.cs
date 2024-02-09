using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Reflection;
using XnaGame.World.Structures;

namespace XnaGame.Content
{
    public static class Structures
    {
        public static Structure test;

        public static void Init(ContentManager content)
        {
            test = new Structure(@"
i: ignore ignore;
a: stone  air;
s: stone  stone;

[
[i,s,s,s,i],
[s,a,a,a,s],
[s,a,a,a,s],
[s,a,a,a,s],
[i,s,s,s,i]
];
");
        }

        public static StructureRef Get<T>(string value) where T : Structure
        {
            FieldInfo t = typeof(Tiles).GetField(value);
            return () =>
            {
                if (cash.TryGetValue(value, out Structure entity))
                    return (T)entity;
                T res = (T)t.GetValue(null);
                cash.Add(value, res);
                return res;
            };
        }

        private static readonly Dictionary<string, Structure> cash = new Dictionary<string, Structure>();
    }

    public delegate Structure StructureRef();
}
