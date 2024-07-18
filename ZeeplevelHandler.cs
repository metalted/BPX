using System;
using System.IO;
using System.Collections.Generic;

namespace BPX
{
    public static class ZeeplevelHandler
    {
        public static ZeeplevelFile LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                Plugin.Instance.LogMessage("No .zeeplevel file exists at path: " + path);
                return null;
            }

            ZeeplevelFile zeeplevel = new ZeeplevelFile(path);
            return zeeplevel;
        }

        public static void SaveToFile(ZeeplevelFile zeeplevel, string path)
        {
            string[] csvContent = zeeplevel.ToCSV();
            File.WriteAllLines(path, csvContent);
        }

        public static ZeeplevelFile CopyZeeplevelFile(ZeeplevelFile fileToCopy)
        {
            string[] csvContent = fileToCopy.ToCSV();
            string fileName = fileToCopy.FileName;
            ZeeplevelFile copied = new ZeeplevelFile(csvContent);
            copied.SetFileName(fileName);
            return copied;
        }

        public static ZeeplevelFile FromBlockProperties(List<BlockProperties> blockProperties)
        {
            ZeeplevelFile zeeplevel = new ZeeplevelFile();
            zeeplevel.ImportBlockProperties(blockProperties);
            return zeeplevel;
        }
    }
}
