using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPX
{
    public static class TeamXMessaging
    {
        public static int GetBlockAllowance()
        {
            // Check if the TeamX plugin is loaded
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue("com.metalted.zeepkist.teamx", out var pluginInfo))
            {
                var teamXPluginInstance = pluginInfo.Instance; // Get the instance of the TeamX plugin

                if (teamXPluginInstance != null)
                {
                    // Use reflection to call the GetBlockAllowance method
                    var methodInfo = teamXPluginInstance.GetType().GetMethod("GetBlockAllowance");
                    if (methodInfo != null)
                    {
                        try
                        {
                            var result = methodInfo.Invoke(teamXPluginInstance, null);
                            if (result is int blockAllowance)
                            {
                                return blockAllowance;
                            }
                        }
                        catch (Exception ex)
                        {
                            //Logger.LogError($"Error invoking GetBlockAllowance: {ex.Message}");
                        }
                    }
                }
            }

            // Default value if TeamX plugin is not found or an error occurs
            return -1;
        }

        public static bool IsTeamXEditor()
        {
            // Check if the TeamX plugin is loaded
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue("com.metalted.zeepkist.teamx", out var pluginInfo))
            {
                var teamXPluginInstance = pluginInfo.Instance; // Get the instance of the TeamX plugin

                if (teamXPluginInstance != null)
                {
                    // Use reflection to call the IsTeamXEditor method
                    var methodInfo = teamXPluginInstance.GetType().GetMethod("IsTeamXEditor");
                    if (methodInfo != null)
                    {
                        try
                        {
                            var result = methodInfo.Invoke(teamXPluginInstance, null);
                            if (result is bool isEditor)
                            {
                                return isEditor;
                            }
                        }
                        catch (Exception ex)
                        {
                            //Logger.LogError($"Error invoking IsTeamXEditor: {ex.Message}");
                        }
                    }
                }
            }

            // Default value if TeamX plugin is not found or an error occurs
            return false;
        }
    }
}
