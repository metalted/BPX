using System;
using UnityEngine;

namespace BPX
{
    public class BPXGizmo : MonoBehaviour
    {
        public enum Axes { All, X, Y, Z, XY, YZ, XZ };
        public Axes currentAxes = Axes.All;
        private int cycleLength = 3;

        public GameObject Xgizmo, Ygizmo, Zgizmo;

        private Vector3 fullScale = Vector3.one * 10f;
        private Vector3 halfScale = Vector3.one * 5f;

        void Start()
        {
            Xgizmo = BPXManager.central.gizmos.Xgizmo.gameObject;
            Ygizmo = BPXManager.central.gizmos.Ygizmo.gameObject;
            Zgizmo = BPXManager.central.gizmos.Zgizmo.gameObject;
        }

        public Vector3 GetCurrentAxes()
        {
            switch (currentAxes)
            {
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
                default:
                    return Vector3.one;
            }
        }

        public void Cycle(bool forward, bool extended)
        {
            int cycleLength = extended ? 7 : 4;
            int currentIndex = (int)currentAxes;

            if (forward)
            {
                currentIndex = (currentIndex + 1) % cycleLength;
            }
            else
            {
                currentIndex = (currentIndex - 1 + cycleLength) % cycleLength;
            }

            currentAxes = (Axes)currentIndex;
            SetGizmoState(currentAxes);
        }

        private void SetGizmoState(Axes selection)
        {
            SetArrow(Xgizmo, IsAxisSelected(selection, Axes.X));
            SetArrow(Ygizmo, IsAxisSelected(selection, Axes.Y));
            SetArrow(Zgizmo, IsAxisSelected(selection, Axes.Z));
        }

        private bool IsAxisSelected(Axes selection, Axes axis)
        {
            return selection == axis || selection == Axes.All ||
                   (axis == Axes.X && (selection == Axes.XY || selection == Axes.XZ)) ||
                   (axis == Axes.Y && (selection == Axes.XY || selection == Axes.YZ)) ||
                   (axis == Axes.Z && (selection == Axes.XZ || selection == Axes.YZ));
        }

        private void SetArrow(GameObject gizmo, bool active)
        {
            if (gizmo == null) return;
            gizmo.transform.localScale = active ? fullScale : halfScale;
            gizmo.transform.localPosition = new Vector3(
                gizmo.name.Contains("X") ? 8f : 0.0f,
                gizmo.name.Contains("Y") ? 8f : 0.0f,
                gizmo.name.Contains("Z") ? 8f : 0.0f);
        }
    }
}
