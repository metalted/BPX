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
                throw new FileNotFoundException($"The file at path {path} does not exist.");
            }

            string[] csvData = File.ReadAllLines(path);
            string fileName = Path.GetFileName(path);
            return new ZeeplevelFile(csvData, fileName);
        }

        public static void SaveToFile(ZeeplevelFile zeeplevel, string path)
        {
            string csvContent = zeeplevel.ToCSV();
            File.WriteAllText(path, csvContent);
        }
    }
}
