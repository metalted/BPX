using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace BPX
{
    public static class BPXManager
    {
        public static LEV_LevelEditorCentral central;
        private static BPXImagingObject imager;

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

        public static List<BlockProperties> GetSelectedBlocks()
        {
            if(central == null)
            {
                return new List<BlockProperties>();
            }
            else
            {
                return central.selection.list;
            }
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

        private static void InitializeImager()
        {
            if (imager == null)
            {
                GameObject imagerObj = new GameObject("BPXImager");
                GameObject.DontDestroyOnLoad(imagerObj);
                imager = imagerObj.AddComponent<BPXImagingObject>();
                imager.Initialize();
            }
        }

        public static void GenerateImage(ZeeplevelFile zeeplevelFile, int imageSize, UnityAction<List<Texture2D>> callback)
        {
            // Ensure the imager is initialized
            InitializeImager();

            // Call CaptureSubject on the imager with the specified parameters
            imager.CaptureSubject(imageSize, zeeplevelFile, callback);
        }

        public static bool InMovementMode()
        {
            if (central == null)
            {
                return false;
            }

            if (central.gizmos.dragButton.isSelected)
            {
                return true;
            }

            return false;
        }

        public static bool InRotateMode()
        {
            if (central == null)
            {
                return false;
            }

            if (central.gizmos.rotateButton.isSelected)
            {
                return true;
            }

            return false;
        }
    }
}
