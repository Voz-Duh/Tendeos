using Rand = System.Random;

namespace XnaGame.Utils
{
    public static class Random
    {
        private static Rand random = Rand.Shared;

        public static int Int() => random.Next();
        public static int Int(int max) => random.Next(max);
        public static int Int(int min, int max) => random.Next(min, max);

        public static float Float() => random.NextSingle();
        public static float Float(float max) => random.NextSingle() * max;
        public static float Float(float min, float max) => min + (random.NextSingle() * (max - min));
    }
}
