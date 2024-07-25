using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BPX
{
    public static class BPXUtils
    {
        #region Maths
        public static BlockPropertyJSON ZeeplevelBlockToBlockPropertyJSON(ZeeplevelBlock block)
        {
            BlockPropertyJSON blockPropertyJSON = new BlockPropertyJSON();
            blockPropertyJSON.position = new Vector3(block.Position.x, block.Position.y, block.Position.z);
            blockPropertyJSON.eulerAngles = new Vector3(block.Rotation.x, block.Rotation.y, block.Rotation.z);
            blockPropertyJSON.localScale = new Vector3(block.Scale.x, block.Scale.y, block.Scale.z);
            blockPropertyJSON.properties = new List<float>();
            foreach(float f in block.Properties)
            {
                blockPropertyJSON.properties.Add(f);
            }
            blockPropertyJSON.blockID = block.BlockID;

            return blockPropertyJSON;
        }

        public static Vector3 ClosestGridPosition(Vector3 position)
        {
            return new Vector3(Mathf.FloorToInt(position.x / 16f), Mathf.FloorToInt(position.y / 16f), Mathf.FloorToInt(position.z / 16f)) * 16f;
        }

        public static Vector3 GetCenterPosition(List<BlockProperties> list)
        {
            Vector3 total = Vector3.zero;

            foreach (BlockProperties bp in list)
            {
                total += bp.gameObject.transform.position;
            }

            return new Vector3(total.x / list.Count, total.y / list.Count, total.z / list.Count);
        }

        public static Vector3[] ConvertLocalToWorldVectors(Transform local)
        {
            Vector3[] xDirections = GetSortedDirections(local.right);
            Vector3[] yDirections = GetSortedDirections(local.up);
            Vector3[] zDirections = GetSortedDirections(local.forward);

            List<string> taken = new List<string>();

            Vector3 chosenXDirection = xDirections[0];
            Vector3 chosenYDirection = Vector3.zero;
            Vector3 chosenZDirection = Vector3.zero;

            taken.Add(GetAxisFromDirection(chosenXDirection));

            foreach (Vector3 vy in yDirections)
            {
                string axis = GetAxisFromDirection(vy);

                if (!taken.Contains(axis))
                {
                    chosenYDirection = vy;
                    taken.Add(axis);
                    break;
                }
            }

            foreach (Vector3 vz in zDirections)
            {
                string axis = GetAxisFromDirection(vz);

                if (!taken.Contains(axis))
                {
                    chosenZDirection = vz;
                    taken.Add(axis);
                    break;
                }
            }

            return new Vector3[] { GetAbsoluteVector(chosenXDirection), GetAbsoluteVector(chosenYDirection), GetAbsoluteVector(chosenZDirection) };
        }

        public static Vector3 GetAbsoluteVector(Vector3 inputVector)
        {
            return new Vector3(Mathf.Abs(inputVector.x), Mathf.Abs(inputVector.y), Mathf.Abs(inputVector.z));
        }

        public static string GetAxisFromDirection(Vector3 direction)
        {
            // Get the absolute values of the direction components
            float absX = Mathf.Abs(direction.x);
            float absY = Mathf.Abs(direction.y);
            float absZ = Mathf.Abs(direction.z);

            // Check which axis has the largest absolute value
            if (absX > absY && absX > absZ)
            {
                return "X";
            }
            else if (absY > absX && absY > absZ)
            {
                return "Y";
            }
            else
            {
                return "Z";
            }
        }

        public static Vector3[] GetSortedDirections(Vector3 inputVector)
        {
            // Convert input vector to world space
            Vector3 inputVectorWorld = inputVector.normalized;

            // Define the six directions
            Vector3[] directions = {
                Vector3.up,
                Vector3.down,
                Vector3.left,
                Vector3.right,
                Vector3.forward,
                Vector3.back
            };

            // Calculate the angles between input vector and the six directions
            float[] angles = new float[directions.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                angles[i] = Mathf.RoundToInt(Vector3.Angle(inputVectorWorld, directions[i]));
            }

            // Sort the directions based on the angles
            System.Array.Sort(angles, directions);

            return directions;
        }

        public static Bounds CalculateBounds(List<GameObject> objs)
        {
            Vector3 minBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 maxBounds = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (GameObject o in objs)
            {
                MeshRenderer[] renderers = o.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer r in renderers)
                {
                    if (r != null)
                    {
                        Bounds b = r.bounds;
                        minBounds = Vector3.Min(minBounds, b.min);
                        maxBounds = Vector3.Max(maxBounds, b.max);
                    }
                }
            }

            Vector3 center = (minBounds + maxBounds) * 0.5f;
            Vector3 size = maxBounds - minBounds;
            return new Bounds(center, size);
        }

        public static Bounds CalculateBounds(List<BlockProperties> bps)
        {
            Vector3 minBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 maxBounds = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (BlockProperties bp in bps)
            {
                MeshRenderer[] renderers = bp.gameObject.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer r in renderers)
                {
                    if (r != null)
                    {
                        Bounds b = r.bounds;
                        minBounds = Vector3.Min(minBounds, b.min);
                        maxBounds = Vector3.Max(maxBounds, b.max);
                    }
                }
            }

            Vector3 center = (minBounds + maxBounds) * 0.5f;
            Vector3 size = maxBounds - minBounds;
            return new Bounds(center, size);
        }
        #endregion

        #region UI
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
        #endregion
    }
}
