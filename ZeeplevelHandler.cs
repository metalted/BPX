using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace BPX
{
    public static class ZeeplevelHandler
    {
        public static ZeeplevelFile LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                Plugin.Instance.LogMessage("No .zeeplevel file exists at path: " + path);
                return null;
            }

            ZeeplevelFile zeeplevel = new ZeeplevelFile(path);
            return zeeplevel;
        }

        public static void InstantiateBlueprintIntoEditor(ZeeplevelFile zeeplevelFile, bool loadHere = true)
        {
            BPXManager.DeselectAllBlocks();

            BPXUndoRedoRegistration registration = new BPXUndoRedoRegistration();
            registration.SetBefore(zeeplevelFile.Blocks.Count);

            for (int i = 0; i < zeeplevelFile.Blocks.Count; i++)
            {
                int id = zeeplevelFile.Blocks[i].BlockID;

                //Skip invalid block ids
                if (id < 0 || id >= PlayerManager.Instance.loader.globalBlockList.blocks.Count)
                {
                    continue;
                }

                //Generate a new UID.
                string newUID = PlayerManager.Instance.GenerateUniqueIDforBlocks(id.ToString());

                //Create a blockPropertyJSON of the ZeeplevelBlock
                BlockPropertyJSON blockPropertyJSON = BPXUtils.ZeeplevelBlockToBlockPropertyJSON(zeeplevelFile.Blocks[i]);

                //Instantiate the block properties.
                BlockProperties bp = BPXManager.central.undoRedo.GenerateNewBlock(blockPropertyJSON, newUID);

                //Add it to the list of blocks.
                registration.blockList.Add(bp);

                //Add a json representation to the after block list.
                registration.after.Add(bp.ConvertBlockToJSON_v15_string());
            }

            registration.GenerateAfter();
            Change_Collection collection = registration.CreateCollection();
            BPXManager.central.validation.BreakLock(collection, "Gizmo6");
            BPXManager.central.selection.UndoRedoReselection(registration.blockList);

            if (loadHere)
            {
                BPXBlueprintBounds bounds = new BPXBlueprintBounds(registration.blockList);
                Vector3 cameraGridPosition = BPXUtils.ClosestGridPosition(BPXManager.central.cam.transform.position);
                Vector3 blueprintGridPosition = BPXUtils.ClosestGridPosition(bounds.bounds.center);
                Vector3 move = cameraGridPosition - blueprintGridPosition;
                BPXOperations.Move(registration.blockList, move);
            }
        }

        public static void SaveToFile(ZeeplevelFile zeeplevel, string path)
        {
            string[] csvContent = zeeplevel.ToCSV();
            File.WriteAllLines(path, csvContent);            
        }

        public static ZeeplevelFile CopyZeeplevelFile(ZeeplevelFile fileToCopy)
        {
            string[] csvContent = fileToCopy.ToCSV();
            string fileName = fileToCopy.FileName;
            ZeeplevelFile copied = new ZeeplevelFile(csvContent);
            copied.SetFileName(fileName);
            return copied;
        }

        public static ZeeplevelFile FromBlockProperties(List<BlockProperties> blockProperties)
        {
            ZeeplevelFile zeeplevel = new ZeeplevelFile();
            zeeplevel.ImportBlockProperties(blockProperties);
            return zeeplevel;
        }
    }
}
