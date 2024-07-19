using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BPX
{
    public class BPXBlueprintBounds
    {
        public Bounds bounds;

        public BPXBlueprintBounds(List<BlockProperties> blockList)
        {
            Vector3 minBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 maxBounds = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (BlockProperties bp in blockList)
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

            bounds = new Bounds((minBounds + maxBounds) * 0.5f, maxBounds - minBounds);
        }
    }
}
