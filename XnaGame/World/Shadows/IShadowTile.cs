
using Microsoft.Xna.Framework.Graphics;
using System;
using XnaGame.Content;
using XnaGame.Utils.Graphics;
using XnaGame.Utils;
using XnaGame.World.Shadows;
using XnaGame.World;

namespace XnaGame.World.Shadows
{
    public interface IShadowTile
    {
        bool ShadowAvailable { get; }
        float ShadowIntensity { get; }
    }
}
