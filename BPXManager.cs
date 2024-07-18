﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPX
{
    public static class BPXManager
    {
        public static LEV_LevelEditorCentral central;

        public static void DeselectAllBlocks()
        {
            if(central == null)
            {
                return;
            }

            central.selection.DeselectAllBlocks(true, nameof(central.selection.ClickNothing));
        }

        public static bool AnyObjectsSelected()
        {
            if(central == null)
            {
                return false;
            }

            return central.selection.list.Count > 0;
        }

        public static string GetPlayerName()
        {
            string playerName = "Bouwerman";

            try
            {
                playerName = PlayerManager.Instance.steamAchiever.GetPlayerName(true);
                return playerName;
            }
            catch
            {
                return playerName;
            }
        }

        public static void InstantiateBlueprintIntoEditor(ZeeplevelFile zeeplevelFile, bool loadHere = true)
        {
            Plugin.Instance.LogMessage("Loading blueprint! " + (loadHere ? "Here!" : " File!"));
        }
    }
}
