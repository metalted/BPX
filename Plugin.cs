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

        public DirectoryInfo pluginDirectory;
        public DirectoryInfo levelDirectory;

        private void Awake()
        {
            pluginPath = AppDomain.CurrentDomain.BaseDirectory + @"\BepInEx\plugins";
            levelPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Zeepkist\\Levels";

            Harmony harmony = new Harmony(pluginGuid);
            harmony.PatchAll();

            RefreshDirectories();
        }

        public void LogMessage(string message)
        {
            Logger.LogInfo(message);
        }

        public void RefreshDirectories()
        {
            pluginDirectory = new DirectoryInfo(pluginPath);
            levelDirectory = new DirectoryInfo(levelPath);
        }
    }

    [HarmonyPatch(typeof(LEV_LevelEditorCentral), "Awake")]
    public class LEVAwake
    {
        public static void Postfix(LEV_LevelEditorCentral __instance)
        {
            UIManagement.InitializeLevelEditor(__instance);
        }
    }
}
