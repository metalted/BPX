using BepInEx;
using HarmonyLib;
using System;

namespace BPX
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "com.metalted.zeepkist.blueprintsX";
        public const string pluginName = "Blueprints X";
        public const string pluginVersion = "2.0";

        public static Plugin Instance;
        public string pluginDirectory;
        public string levelDirectory;

        private void Awake()
        {
            pluginDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\BepInEx\plugins";
            levelDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Zeepkist\\Levels";

            Harmony harmony = new Harmony(pluginGuid);
            harmony.PatchAll();
        }

        public void LogMessage(string message)
        {
            Logger.LogInfo(message);
        }
    }
}
