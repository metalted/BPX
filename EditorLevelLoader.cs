using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BPX
{
    public static class EditorLevelLoader
    {
        public static void LoadToEditor(ZeeplevelData data, bool loadHere = true)
        {
            int count = data.json != null ? data.json.blox.Count : data.csv.Blocks.Count;
            if(count == 0)
            {
                return;
            }

            if (TeamXMessaging.IsTeamXEditor())
            {
                int allowance = TeamXMessaging.GetBlockAllowance();
                
                if (count >= allowance)
                {
                    PlayerManager.Instance.messenger.Log("Blueprint exceeds blocks limit!", 2f);
                    return;
                }
            }

            if (data == null || !data.Valid)
            {
                Debug.LogError("EditorLevelLoader: Invalid ZeeplevelData");
                return;
            }

            BPXManager.DeselectAllBlocks();

            List<BlockProperties> blockList = new List<BlockProperties>();

            if (data.json != null)
            {
                blockList = LoadV15Blocks(data.json);
            }
            else if (data.csv != null)
            {
                blockList = LoadV14Blocks(data.csv);
            }
            else
            {
                Debug.LogError("EditorLevelLoader: No block data found.");
                return;
            }            

            BPXUndoRedoRegistration registration = new BPXUndoRedoRegistration();
            registration.SetBefore(count);
            registration.blockList.AddRange(blockList);
            registration.after.AddRange(blockList.Select(bp => bp.ConvertBlockToJSON_v15_string(true)).ToList());

            registration.GenerateAfter();
            Change_Collection collection = registration.CreateCollection();
            BPXManager.central.validation.BreakLock(collection, "Gizmo6");
            BPXManager.central.selection.UndoRedoReselection(registration.blockList);

            if (loadHere)
            {
                Bounds bounds = BPXUtils.CalculateBounds(registration.blockList);
                Vector3 cameraGridPosition = BPXUtils.ClosestGridPosition(BPXManager.central.cam.transform.position);
                Vector3 blueprintGridPosition = BPXUtils.ClosestGridPosition(bounds.center);
                Vector3 move = cameraGridPosition - blueprintGridPosition;
                BPXOperations.Move(registration.blockList, move);
            }
        }

        public static List<BlockProperties> LoadV15Blocks(v15LevelJSON json)
        {
            List<BlockProperties> blocks = new List<BlockProperties>();
            foreach (var blockJson in json.blox)
            {
                if (blockJson.i < 0 || blockJson.i >= PlayerManager.Instance.loader.globalBlockList.blocks.Count)
                    continue;

                var prefab = PlayerManager.Instance.loader.globalBlockList.blocks[blockJson.i];
                var instance = UnityEngine.Object.Instantiate(prefab);
                instance.name = prefab.name;
                instance.isEditor = true;
                instance.CreateBlock();
                instance.properties.Clear();
                instance.LoadProperties_v15(blockJson, true);
                blocks.Add(instance);
            }

            Debug.Log($"Loaded {json.blox.Count} v15 blocks into editor.");
            return blocks;
        }

        public static List<BlockProperties> LoadV14Blocks(v14LevelCSV csv)
        {
            List<BlockProperties> blocks = new List<BlockProperties>();
            foreach (var blockData in csv.Blocks)
            {
                if (!blockData.Valid || blockData.BlockID < 0 || blockData.BlockID >= PlayerManager.Instance.loader.globalBlockList.blocks.Count)
                    continue;

                var prefab = PlayerManager.Instance.loader.globalBlockList.blocks[blockData.BlockID];
                BlockProperties instance = UnityEngine.Object.Instantiate(prefab);
                instance.name = prefab.name;
                instance.isEditor = true;
                instance.CreateBlock();
                instance.properties.Clear();
                instance.UID = PlayerManager.Instance.GenerateUniqueIDforBlocks(blockData.BlockID.ToString());
                instance.properties.AddRange(blockData.Properties);
                instance.transform.localPosition = blockData.Position;
                instance.transform.localEulerAngles = blockData.Rotation;
                instance.transform.localScale = blockData.Scale;
                instance.LoadProperties();
                blocks.Add(instance);
            }

            Debug.Log($"Loaded {csv.Blocks.Count} v14 blocks into editor.");
            return blocks;
        }
    }

}
