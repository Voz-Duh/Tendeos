using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Tendeos.World.Generation;

namespace Tendeos.World
{
    public interface IChunk
    {
        Biome Biome { get; set; }
        List<Rectangle> AirQuadtree { get; }
    }
}