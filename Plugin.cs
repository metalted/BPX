using BepInEx;
using HarmonyLib;
using System;
using System.IO;

namespace BPX
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "com.metalted.zeepkist.blueprintsX";
        public const string pluginName = "Blueprints X";
        public const string pluginVersion = "2.1.7";

        public static Plugin Instance;
        public string pluginPath;
        public string blueprintPath;
        public string levelPath;

        public static bool addSDKTooltips = false;

        private void Awake()
        {
            pluginPath = AppDomain.CurrentDomain.BaseDirectory + @"\BepInEx\plugins";
            blueprintPath = Path.Combine(pluginPath, "Blueprints");

            if(!Directory.Exists(blueprintPath))
            {
                Directory.CreateDirectory(blueprintPath);
            }
            
            levelPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Zeepkist\\Levels";
            Instance = this;

            BPXConfiguration.Initialize(Config);
            BPXSprites.Initialize();

            Harmony harmony = new Harmony(pluginGuid);
            harmony.PatchAll();
        }

        public void LogMessage(string message)
        {
            Logger.LogInfo(message);
        }

        public void LogScreenMessage(string message)
        {
            PlayerManager.Instance.messenger.Log(message, 2.5f);
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
}
