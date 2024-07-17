using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPX
{
    public static class BPXHistory
    {
        private static ZeeplevelFile previouslyLoadedBlueprint = null;

        public static bool PreviouslyLoadedBlueprintAvailable()
        {
            return false;
        }
    }
}
