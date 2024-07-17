using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPX
{
    public static class BPXConfiguration
    {
        public static bool DoubleLoadButton()
        {
            return true;
        }

        private static List<string> allowedExtensions = new List<string>() { ".png", ".obj", ".jpg", ".realm", ".zeeplist", ".zip", ".customsoapbox" };
        public static bool IsAllowedExtension(string ext)
        {
            return allowedExtensions.Contains(ext);
        }
    }
}
