using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace BPX
{
    public static class BPXImager
    {
        private static BPXImagingObject imager;

        public static void Initialize()
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
            Initialize();

            // Define a tag for the callback (it can be any unique string)
            string tag = System.Guid.NewGuid().ToString();

            // Call CaptureSubject on the imager with the specified parameters
            imager.CaptureSubject(imageSize, zeeplevelFile, tag, callback);
        }
    }
}
