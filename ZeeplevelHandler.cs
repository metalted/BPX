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

            string[] csvData = File.ReadAllLines(path);
            string fileName = Path.GetFileName(path);
            return new ZeeplevelFile(csvData, fileName);
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

            ZeeplevelFile copied = new ZeeplevelFile(csvContent, fileName);
            return copied;
        }

        public static ZeeplevelFile BlockPropertiesToZeeplevelFile(List<BlockProperties> blockProperties)
        {
            List<ZeeplevelBlock> blocks = new List<ZeeplevelBlock>();
            foreach(BlockProperties bp in  blockProperties)
            {
                ZeeplevelBlock block = new ZeeplevelBlock(bp);
                blocks.Add(block);
            }

            ZeeplevelFile file = new ZeeplevelFile(blocks);
            return file;
        }
    }
}
