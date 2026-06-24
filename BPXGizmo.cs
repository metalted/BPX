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

            currentAxes = (Axis)currentIndex;
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
            if(axis == Axis.XYZ)
            {
                return false;
            }

            return selection == axis || 
                   (axis == Axis.X && (selection == Axis.XY || selection == Axis.XZ)) ||
                   (axis == Axis.Y && (selection == Axis.XY || selection == Axis.YZ)) ||
                   (axis == Axis.Z && (selection == Axis.XZ || selection == Axis.YZ));
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
