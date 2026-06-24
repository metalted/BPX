using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BPX.UI;

namespace BPX
{
    public static class BPXUtils
    {
        public static Vector3 WorldSpaceRelativeMovement(Vector3 lookDirection, Vector3 move)
        {
            Vector3 relativeMove = Vector3.zero;

            if (Mathf.Abs(lookDirection.x) <= Mathf.Abs(lookDirection.z))
            {
                if (Mathf.Sign(lookDirection.z) < 0)
                {
                    relativeMove.x = move.x * -1;
                    relativeMove.y = move.y;
                    relativeMove.z = move.z * -1;
                }
                else
                {
                    relativeMove = move;
                }
            }
            else
            {
                if (Mathf.Sign(lookDirection.x) < 0)
                {
                    relativeMove.x = move.z * -1;
                    relativeMove.y = move.y;
                    relativeMove.z = move.x;
                }
                else
                {
                    relativeMove.x = move.z;
                    relativeMove.y = move.y;
                    relativeMove.z = move.x * -1;
                }
            }
            return relativeMove;
        }

        public static void DrawScreenRect(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, BPXSprites.whiteTexture);
            GUI.color = Color.white;
        }

        public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
        {
            // Top
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            // Left
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            // Right
            DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
            // Bottom
            DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
        }

        public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
        {
            // Move origin from bottom left to top left
            screenPosition1.y = Screen.height - screenPosition1.y;
            screenPosition2.y = Screen.height - screenPosition2.y;
            // Calculate corners
            var topLeft = Vector3.Min(screenPosition1, screenPosition2);
            var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
            // Create Rect
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }

        public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
        {
            var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
            var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
            var min = Vector3.Min(v1, v2);
            var max = Vector3.Max(v1, v2);
            min.z = camera.nearClipPlane;
            max.z = camera.farClipPlane;

            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }
    }
}
