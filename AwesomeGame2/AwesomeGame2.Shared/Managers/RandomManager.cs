using System;
using AwesomeGame2.Shared.Saves;

namespace AwesomeGame2.Shared.Managers
{
    public sealed class RandomManager
    {
        public int Next(GameSave save, int minInclusive, int maxExclusive)
        {
            if (save == null)
            {
                throw new ArgumentNullException(nameof(save));
            }

            if (maxExclusive <= minInclusive)
            {
                throw new ArgumentException("Invalid random range.");
            }

            unchecked
            {
                var value = save.RandomState.Seed;
                value = (value * 1103515245 + 12345 + save.RandomState.Calls) & int.MaxValue;
                save.RandomState.Seed = value;
                save.RandomState.Calls += 1;
                var range = maxExclusive - minInclusive;
                return minInclusive + (value % range);
            }
        }
    }
}
