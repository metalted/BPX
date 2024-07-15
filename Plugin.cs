using BepInEx;
using HarmonyLib;


namespace BPX
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "com.metalted.zeepkist.blueprintsX";
        public const string pluginName = "Blueprints X";
        public const string pluginVersion = "2.0";
        public static Plugin Instance;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
