using Tendeos.Modding;
using Tendeos.World.Structures;

namespace Tendeos.Content
{
    public static class Structures
    {
        public static Structure test;

        public static void Init()
        {
            test = new Structure(MIS.GenerateVirtual(@"
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
", "Virtual/test.mis"));
        }

        public static Structure Get(string value) => (Structure) typeof(Structures).GetField(value).GetValue(null);
    }
}