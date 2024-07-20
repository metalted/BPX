using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPX
{
    public static class BPXConfiguration
    {
        private static float[] gizmoScaleValues = new float[] { 0.05f, 0.5f, 1f, 2f, 10f, 20f, 50f };

        public static bool DoubleLoadButton()
        {
            return true;
        }

        private static List<string> allowedExtensions = new List<string>() { ".png", ".obj", ".jpg", ".realm", ".zeeplist", ".zip", ".customsoapbox" };
        public static bool IsAllowedExtension(string ext)
        {
            return allowedExtensions.Contains(ext);
        }

        public static float[] GetScalingValues()
        {
            return gizmoScaleValues;
        }
    }
}
