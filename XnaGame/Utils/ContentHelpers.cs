using Microsoft.Xna.Framework.Content;
using System.IO;

namespace XnaGame.Utils
{
    public static class ContentHelpers
    {
        public static byte[] LoadFileBytes(this ContentManager content, string filePath)
        {
            string fullPath = Path.Combine(content.RootDirectory, filePath);
            return File.ReadAllBytes(fullPath);
        }
    }
}
