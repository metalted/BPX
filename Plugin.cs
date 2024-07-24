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
        public const string pluginVersion = "2.0";

        public static Plugin Instance;
        public string pluginPath;
        public string levelPath;        

        private void Awake()
        {
            pluginPath = AppDomain.CurrentDomain.BaseDirectory + @"\BepInEx\plugins";
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
            PlayerManager.Instance.messenger.Log(message, 1f);
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
