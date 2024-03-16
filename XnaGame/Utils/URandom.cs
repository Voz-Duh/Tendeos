using Rand = System.Random;

namespace XnaGame.Utils
{
    public class URandom
    {
        private static Rand random = Rand.Shared;

        public static int SInt() => random.Next();
        public static int SInt(int max) => random.Next(max);
        public static int SInt(int min, int max) => random.Next(min, max);

        public static float SFloat() => random.NextSingle();
        public static float SFloat(float max) => random.NextSingle() * max;
        public static float SFloat(float min, float max) => min + (random.NextSingle() * (max - min));

        private Rand privateRandom;

        public URandom(uint seed)
        {
            privateRandom = new Rand((int)(seed + int.MinValue));
        }

        public int Int() => privateRandom.Next();
        public int Int(int max) => privateRandom.Next(max);
        public int Int(int min, int max) => privateRandom.Next(min, max);

        public float Float() => privateRandom.NextSingle();
        public float Float(float max) => privateRandom.NextSingle() * max;
        public float Float(float min, float max) => min + (privateRandom.NextSingle() * (max - min));
    }
}
