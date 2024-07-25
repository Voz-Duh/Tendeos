using System;

namespace Tendeos.Utils
{
    public static class RandomHelper
    {
        public static int Random(this Range value) =>
            URandom.SInt(value.Start.IsFromEnd ? 0 : value.Start.Value,
                value.End.IsFromEnd ? 0 : (value.End.Value + 1));
    }
}