using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPX
{
    public static class BPXOnline
    {
        public static void Search(string query)
        {
            Plugin.Instance.LogScreenMessage("Searching for " + query + "...");
        }
    }
}
