using System.Collections.Generic;

namespace AwesomeGame2.Shared.Models
{
    public sealed class ProgressionFlags
    {
        public HashSet<string> Flags { get; set; }

        public ProgressionFlags()
        {
            Flags = new HashSet<string>();
        }
    }
}
