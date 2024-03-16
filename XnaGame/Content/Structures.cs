using Microsoft.Xna.Framework.Content;
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

        public static Structure Get(string value) => (Structure)typeof(Structures).GetField(value).GetValue(null);
    }
}
