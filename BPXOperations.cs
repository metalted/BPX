using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BPX
{
    public static class BPXOperations
    {
        public static void Move(List<BlockProperties> blockList, Vector3 direction)
        {
            BPXUndoRedoRegistration registration = new BPXUndoRedoRegistration();
            registration.SetBefore(blockList);

            //Move each block
            foreach (BlockProperties bp in blockList)
            {
                bp.transform.position += direction;
                bp.SomethingChanged();
            }

            // Move the mother gizmo by the specified amount
            BPXManager.central.gizmos.motherGizmo.position += direction;

            registration.GenerateAfter();

            Change_Collection collection = registration.CreateCollection();
            BPXManager.central.validation.BreakLock(collection, "Gizmo1");
        }
    }
}
