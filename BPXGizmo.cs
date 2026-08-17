using System;
using UnityEngine;
using Toolkist;

namespace BPX
{
    public class BPXGizmo : MonoBehaviour
    {
        public Axis currentAxes = Axis.XYZ;

        public GameObject Xgizmo, Ygizmo, Zgizmo;

        private Vector3 fullScale = Vector3.one * 10f;
        private Vector3 halfScale = Vector3.one * 5f;

        void Start()
        {
            Xgizmo = BPXManager.central.gizmos.Xgizmo.gameObject;
            Ygizmo = BPXManager.central.gizmos.Ygizmo.gameObject;
            Zgizmo = BPXManager.central.gizmos.Zgizmo.gameObject;

            SetGizmoState(currentAxes);
        }

        public void Reset()
        {
            currentAxes = Axis.XYZ;
            SetGizmoState(currentAxes);
        }

        public Axis GetCurrent()
        {
            return currentAxes;
        }

        public void Cycle(bool forward, bool extended)
        {
            Axis[] cycle = extended
                ? new Axis[]
                {
                    Axis.XYZ,
                    Axis.X,
                    Axis.Y,
                    Axis.Z,
                    Axis.XY,
                    Axis.YZ,
                    Axis.XZ
                }
                : new Axis[]
                {
                    Axis.XYZ,
                    Axis.X,
                    Axis.Y,
                    Axis.Z
                };

            int currentIndex = Array.IndexOf(cycle, currentAxes);

            if (currentIndex == -1)
            {
                currentIndex = 0;
            }

            if (forward)
            {
                currentIndex = (currentIndex + 1) % cycle.Length;
            }
            else
            {
                currentIndex = (currentIndex - 1 + cycle.Length) % cycle.Length;
            }

            currentAxes = cycle[currentIndex];

            SetGizmoState(currentAxes);
        }

        private void SetGizmoState(Axis selection)
        {
            SetArrow(Xgizmo, IsAxisSelected(selection, Axis.X));
            SetArrow(Ygizmo, IsAxisSelected(selection, Axis.Y));
            SetArrow(Zgizmo, IsAxisSelected(selection, Axis.Z));
        }

        private bool IsAxisSelected(Axis selection, Axis axis)
        {
            return selection == axis ||
                   (axis == Axis.X && (selection == Axis.XY || selection == Axis.XZ)) ||
                   (axis == Axis.Y && (selection == Axis.XY || selection == Axis.YZ)) ||
                   (axis == Axis.Z && (selection == Axis.XZ || selection == Axis.YZ));
        }

        private void SetArrow(GameObject gizmo, bool active)
        {
            if (gizmo == null)
            {
                return;
            }

            gizmo.transform.localScale = active ? fullScale : halfScale;

            gizmo.transform.localPosition = new Vector3(
                gizmo.name.Contains("X") ? 8f : 0.0f,
                gizmo.name.Contains("Y") ? 8f : 0.0f,
                gizmo.name.Contains("Z") ? 8f : 0.0f);
        }
    }
}