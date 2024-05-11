using Microsoft.Xna.Framework.Content;
using Tendeos.World.Structures;

namespace Tendeos.Content
{
    public static class Structures
    {
        public static Structure test;

        public static void Init(ContentManager content)
        {
            test = new Structure(@"
i: [ignore, ignore];
s: [stone, stone];
w: [stone, air];

struct: [
    [i, s, s, s, i],
    [s, w, w, w, s],
    [s, w, w, w, s],
    [s, w, w, w, s],
    [i, s, s, s, i]
];
");
        }

        public static Structure Get(string value) => (Structure)typeof(Structures).GetField(value).GetValue(null);
    }
}
