using BepInEx;
using HarmonyLib;
using System;
using System.IO;
using BPX.UI;
using UnityEngine;
using UnityEngine.Events;
using FMODSyntax;

namespace BPX
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInDependency("ZeepSDK", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.metalted.zeepkist.toolkist", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.metalted.zeepkist.teamx", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "com.metalted.zeepkist.blueprintsX";
        public const string pluginName = "Blueprints X";
        public const string pluginVersion = "4.1";

        private bool logging_enabled = false;

        public static Plugin Instance;

        private void Awake()
        {
            Instance = this;

            BPXConfiguration.Initialize(Config);
            BPXSprites.Initialize();

            Harmony harmony = new Harmony(pluginGuid);
            harmony.PatchAll();
        }

        public void LogMessage(string message)
        {
            if(!logging_enabled)
            {
                return;
            }

            Logger.LogInfo(message);
        }

        public void LogScreenMessage(string message)
        {
            PlayerManager.Instance.messenger.LogCustomColor(message, 2.5f, Color.white, BPXUIManagement.darkBlue);
        }

        public void LogScreenErrorMessage(string message)
        {
            PlayerManager.Instance.messenger.LogCustomColor(message, 2.5f, Color.white, BPXUIManagement.red);
        }
    }

    [HarmonyPatch(typeof(LEV_LevelEditorCentral), "Awake")]
    public class LEVAwake
    {
        public static void Postfix(LEV_LevelEditorCentral __instance)
        {
            BPXManager.central = __instance;
            BPXUIManagement.InitializeLevelEditor(__instance);
        }
    }

    [HarmonyPatch(typeof(LEV_Selection), "DeselectAllBlocks")]
    public class Selection_DeselectAll
    {
        public static void Postfix()
        {
            BPXGizmo gizmo = BPXUIManagement.GetGizmo();
            if(gizmo != null)
            {
                gizmo.Reset();
            }            
        }
    }

    [HarmonyPatch(typeof(SkyboxManager), "PreviousCurrent")]
    public class SkyboxManager_PreviousCurrent
    {
        public static bool Prefix(SkyboxManager __instance)
        {
            if (BPXUIManagement.IsPanelOpen())
            {
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(SkyboxManager), "AdvanceCurrent")]
    public class SkyboxManager_AdvanceCurrent
    {
        public static bool Prefix(SkyboxManager __instance)
        {
            if (BPXUIManagement.IsPanelOpen())
            {
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(LEV_Inspector), "CreateTreeGUI")]
    public static class LEV_Inspector_CreateTreeGUI_Patch
    {
        private static void Postfix(LEV_Inspector __instance)
        {
            if (__instance == null)
                return;

            // Only add this in the top-level tree folder.
            if (__instance.currentBlocksFolder == null)
                return;

            if (__instance.currentBlocksFolder.hasParent)
                return;

            // Only add this when a blueprint has been selected.
            if (BPXManager.treegunBlueprint == null)
                return;

            AddTreeGunBlueprintButton(__instance);
        }

        private static void AddTreeGunBlueprintButton(LEV_Inspector gui)
        {
            int index = gui.allButtons2.Count;

            ThumbnailButton thumbnailButton = GameObject.Instantiate(gui.buttonPrefab2);

            thumbnailButton.name = "BPX TreeGun Blueprint Button";
            thumbnailButton.button.central = gui.central;

            gui.blockThumbImages.Add(thumbnailButton.thumbnailImage);

            if (BPXManager.treegunBlueprintSprite != null)
                thumbnailButton.thumbnailImage.sprite = BPXManager.treegunBlueprintSprite;

            thumbnailButton.button.onClick.AddListener((UnityAction)(() =>
            {
                BPXManager.OnTreeGunBlueprintButton();

                gui.PositionIndicator(
                    index,
                    true,
                    "Blueprint"
                );
            }));

            thumbnailButton.button.onHoverEnter.AddListener((UnityAction)(() =>
            {
                gui.inspectorTitle.text = "Blueprint";
            }));

            thumbnailButton.button.onHoverExit.AddListener((UnityAction)(() =>
            {
                gui.BlockHoverExit();
            }));

            thumbnailButton.ApplyIcon(null);

            thumbnailButton.transform.SetParent(gui.contentBox, false);

            RectTransform component = thumbnailButton.GetComponent<RectTransform>();

            int horizontalAmount = gui.GetHorizontalAmount();

            int layoutAmount = gui.allButtons2.Count;
            float num4 = Mathf.Ceil((float)(layoutAmount / horizontalAmount));

            float num5 = Mathf.Floor((float)index / (float)horizontalAmount);
            float num7 = 1f / (num4 + 1f);

            float y1 = 1f - num7 * num5;
            float y2 = y1 - num7;

            float num8 = index % horizontalAmount;
            float num9 = 1f / horizontalAmount;

            float x1 = num9 * num8;
            float x2 = x1 + num9;

            component.anchorMin = new Vector2(x1, y2);
            component.anchorMax = new Vector2(x2, y1);

            gui.allButtons2.Add(thumbnailButton);

            gui.UpdateButtonAspects();

            gui.currentBlockIndicator.SetSiblingIndex(99999);
            gui.currentBlockIndicator.gameObject.SetActive(true);
        }
    }

    [HarmonyPatch(typeof(LEV_TREEGUNNN), "UpdateCurrentTree")]
    public static class LEV_TREEGUNNN_UpdateCurrentTree_Patch
    {
        private static void Postfix(LEV_TREEGUNNN __instance)
        {
            BPXManager.ApplyTreeGunBlueprintTexture(__instance);
        }
    }

    [HarmonyPatch(typeof(LEV_TREEGUNNN), "SetToTree")]
    public static class LEV_TreeGun_SetToTree_Patch
    {
        private static void Prefix()
        {
            BPXManager.isUsingTreegunBlueprint = false;
        }
    }

    [HarmonyPatch(typeof(LEV_TREEGUNNN), "CreateNewPipetteBlock")]
    public static class LEV_TREEGUNNN_CreateNewPipetteBlock_Patch
    {
        private static void Prefix()
        {
            BPXManager.isUsingTreegunBlueprint = false;
        }
    }

    [HarmonyPatch(typeof(LEV_TREEGUNNN), "PlantTree")]
    public static class LEV_TREEGUNNN_PlantTree_Patch
    {
        private static bool Prefix(LEV_TREEGUNNN __instance, ref Vector3 point, ref BlockProperties blockToPlant, ref Vector3 hitNormal)
        {
            if (!BPXManager.isUsingTreegunBlueprint)
                return true;

            if (BPXManager.treegunBlueprint == null)
                return true;

            BPXManager.PlaceTreegunBlueprint(point, hitNormal, __instance.fieldSize);
            return false;
        }
    }

    [HarmonyPatch(typeof(LEV_TREEGUNNN), "SpawnParticle")]
    public static class LEV_TREEGUNNN_SpawnParticle_Patch
    {
        private static bool Prefix(LEV_TREEGUNNN __instance)
        {
            if(!BPXManager.isUsingTreegunBlueprint)
            {
                return true;
            }

            return false;
        }
    }
}
