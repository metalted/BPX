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
        private static readonly Vector3[] directions = { Vector3.right, Vector3.up, Vector3.forward };

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

        public static void RotateSelection(Vector3 rotation, float angle)
        {
            // Check if there are selected blocks
            if (BPXManager.central.selection.list.Count > 0)
            {
                BPXUndoRedoRegistration registration = new BPXUndoRedoRegistration();
                List<BlockProperties> blockList = BPXManager.central.selection.list;

                registration.SetBefore(blockList);

                //Rotate the selected blocks
                BPXManager.central.gizmos.DoRotate(rotation, angle);

                // Update each blocks properties
                foreach (BlockProperties bp in BPXManager.central.selection.list)
                {
                    bp.SomethingChanged();
                }

                // Convert the blockList to JSON after moving
                registration.GenerateAfter();

                Change_Collection collection = registration.CreateCollection();
                BPXManager.central.validation.BreakLock(collection, "Gizmo11");
            }
        }

        public static void Scale(List<BlockProperties> blockList, Vector3 axis, float amount)
        {
            BPXUndoRedoRegistration registration = new BPXUndoRedoRegistration();
            registration.SetBefore(blockList);

            Vector3 center = BPXUtils.GetCenterPosition(blockList);

            if (axis.x > 0 && axis.y > 0 && axis.z > 0)
            {
                // Scale uniformly
                foreach (BlockProperties bp in blockList)
                {
                    // Calculate the new position and scale of the block
                    Vector3 pos = bp.transform.position;
                    pos -= center;
                    pos *= amount;
                    pos += center;
                    bp.transform.position = pos;
                    bp.transform.localScale *= amount;
                    bp.SomethingChanged();
                }
            }
            else
            {
                foreach (BlockProperties bp in blockList)
                {
                    // Calculate the new position of the block
                    Vector3 pos = bp.transform.position;
                    pos -= center;
                    pos.x = axis.x > 0 ? pos.x * amount : pos.x;
                    pos.y = axis.y > 0 ? pos.y * amount : pos.y;
                    pos.z = axis.z > 0 ? pos.z * amount : pos.z;
                    pos += center;
                    bp.transform.position = pos;

                    Vector3[] convertedVectors = BPXUtils.ConvertLocalToWorldVectors(bp.transform);

                    for (int i = 0; i < 3; i++)
                    {
                        if (axis[i] <= 0)
                        {
                            continue;
                        }

                        Vector3 scaledAxis = Vector3.zero;

                        if (convertedVectors[0] == directions[i])
                        {
                            scaledAxis = Vector3.right;
                        }
                        else if (convertedVectors[1] == directions[i])
                        {
                            scaledAxis = Vector3.up;
                        }
                        else if (convertedVectors[2] == directions[i])
                        {
                            scaledAxis = Vector3.forward;
                        }

                        Vector3 scaleAddition = Vector3.Scale((bp.transform.localScale * amount - bp.transform.localScale), scaledAxis);
                        bp.transform.localScale += scaleAddition;
                    }

                    bp.SomethingChanged();
                }
            }

            registration.GenerateAfter();

            Change_Collection collection = registration.CreateCollection();
            BPXManager.central.validation.BreakLock(collection, "Gizmo1");
        }

        public static void ScaleInPlace(List<BlockProperties> blockList, Vector3 axis, float amount)
        {
            BPXUndoRedoRegistration registration = new BPXUndoRedoRegistration();
            registration.SetBefore(blockList);

            if (axis.x > 0 && axis.y > 0 && axis.z > 0)
            {
                // Scale uniformly
                foreach (BlockProperties bp in blockList)
                {
                    // Scale the block uniformly
                    bp.transform.localScale *= amount;
                    bp.SomethingChanged();
                }
            }
            else
            {
                foreach(BlockProperties bp in blockList)
                {
                    Vector3[] convertedVectors = BPXUtils.ConvertLocalToWorldVectors(bp.transform);

                    for (int i = 0; i < 3; i++)
                    {
                        if (axis[i] <= 0)
                        {
                            continue;
                        }

                        Vector3 scaledAxis = Vector3.zero;

                        if (convertedVectors[0] == directions[i])
                        {
                            scaledAxis = Vector3.right;
                        }
                        else if (convertedVectors[1] == directions[i])
                        {
                            scaledAxis = Vector3.up;
                        }
                        else if (convertedVectors[2] == directions[i])
                        {
                            scaledAxis = Vector3.forward;
                        }

                        Vector3 scaleAddition = Vector3.Scale((bp.transform.localScale * amount - bp.transform.localScale), scaledAxis);
                        bp.transform.localScale += scaleAddition;
                    }

                    bp.SomethingChanged();
                }                
            }

            registration.GenerateAfter();

            Change_Collection collection = registration.CreateCollection();
            BPXManager.central.validation.BreakLock(collection, "Gizmo1");
        }

        public static void Mirror(List<BlockProperties> blockList, Vector3 axis)
        {
            BPXUndoRedoRegistration registration = new BPXUndoRedoRegistration();
            registration.SetBefore(blockList);

            // Calculate the center position of the blockList
            Vector3 center = BPXUtils.GetCenterPosition(blockList);

            // Create a temporary parent object for mirroring
            Transform tempParent = new GameObject("Temp Mirror Parent").transform;
            tempParent.position = center;

            foreach (BlockProperties bp in blockList)
            {
                // Set the temporary parent as the parent of each block
                bp.transform.parent = tempParent;
            }

            // Apply mirroring based on the specified axis
            if (axis.x > 0)
            {
                tempParent.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (axis.y > 0)
            {
                tempParent.transform.localScale = new Vector3(1, -1, 1);
            }
            else if (axis.z > 0)
            {
                tempParent.transform.localScale = new Vector3(1, 1, -1);
            }

            foreach (BlockProperties bp in blockList)
            {
                // Remove the temporary parent by setting each block's parent to null
                bp.transform.parent = null;
                bp.SomethingChanged();
            }

            // Destroy the temporary parent object
            GameObject.Destroy(tempParent.gameObject);

            registration.GenerateAfter();

            Change_Collection collection = registration.CreateCollection();
            BPXManager.central.validation.BreakLock(collection, "Gizmo1");
        }
    }
}
