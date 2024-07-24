using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BPX
{
    public class BPXGizmo : MonoBehaviour
    {
        public enum Axes { All, X, Y, Z, XY, YZ, XZ };
        public Axes currentAxes = Axes.All;

        public Vector3 GetCurrentAxes()
        {
            switch (currentAxes)
            {
                default:
                    case Axes.All:
                    return Vector3.one;
                case Axes.X:
                    return Vector3.right;
                case Axes.Y:
                    return Vector3.up;
                case Axes.Z:
                    return Vector3.forward;
                case Axes.XY:
                    return Vector3.right + Vector3.up;
                case Axes.YZ:
                    return Vector3.up + Vector3.forward;
                case Axes.XZ:
                    return Vector3.right + Vector3.forward;
            }
        }
    }
}
