using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Toolkist;

namespace BPX
{
    public static class BPXManager
    {
        public static LEV_LevelEditorCentral central;
        private static ZeeplevelData clipboardContent;
        public static Vector3 positionClipboard;
        public static Vector3 rotationClipboard;
        public static Vector3 scaleClipboard;
        public static List<float> optionsClipboard;
        public static List<float> paintsClipboard;

        public static ZeeplevelData treegunBlueprint;
        public static Sprite treegunBlueprintSprite;
        public static bool isUsingTreegunBlueprint;

        public static void SetClipboard(ZeeplevelData content)
        {
            clipboardContent = content;
        }

        public static ZeeplevelData GetClipboard()
        {
            return clipboardContent;
        }

        public static void SetTreegunBlueprint(ZeeplevelData data, Sprite img)
        {
            treegunBlueprint = data;
            treegunBlueprintSprite = ZeeplevelImager.MakeImagerBackgroundTransparent(img);
        }

        public static void OnTreeGunBlueprintButton()
        {
            if (treegunBlueprint == null)
            {
                Plugin.Instance.LogMessage("[BPX] Tried to select treegun blueprint, but treegunBlueprint is null.");
                return;
            }

            isUsingTreegunBlueprint = true;

            if (central == null || central.TREEGUNNNN == null)
            {
                Plugin.Instance.LogMessage("[BPX] Could not update treegun display because central or TREEGUNNNN is null.");
                return;
            }

            ApplyTreeGunBlueprintTexture(central.TREEGUNNNN);
        }

        public static void ApplyTreeGunBlueprintTexture(LEV_TREEGUNNN treeGun)
        {
            if (!isUsingTreegunBlueprint)
                return;

            if (treeGun == null)
                return;

            if (treeGun.display == null)
                return;

            if (treegunBlueprintSprite == null)
                return;

            Texture texture = treegunBlueprintSprite.texture;

            if (treeGun.display.material.mainTexture != texture)
                treeGun.display.material.mainTexture = texture;
        }

        public static void PlaceTreegunBlueprint(Vector3 point, Vector3 hitNormal, float size)
        {
            if (treegunBlueprint == null)
                return;

            if (central == null)
                return;

            if (hitNormal == Vector3.zero)
                return;

            List<BlockProperties> blocks = ZeeplevelHandler.LoadIntoEditor(treegunBlueprint, central);

            if (blocks == null || blocks.Count == 0)
                return;

            float angle = 0f;
            if(BPXConfiguration.UseRandomTreegunRotation())
            {
                angle = UnityEngine.Random.Range(0f, 360f);
            }
            else
            {
                angle = Mathf.Repeat(BPXConfiguration.GetTreegunObjectRotation(), 360f);
            }

            EditorOperations.PlaceBlueprint(
                central,
                blocks,
                point,
                !central.TREEGUNNNN.normalUp ? Vector3.up : hitNormal,
                size,
                BPXConfiguration.GetTreegunLiftFactor(),
                angle
            );

            EditorOperations.DeselectAllBlocks(central);
        }
    }
}
