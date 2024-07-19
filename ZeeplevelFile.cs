using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace BPX
{
    public class ZeeplevelFile
    {
        public ZeeplevelHeader Header { get; private set; }
        public List<ZeeplevelBlock> Blocks { get; private set; }
        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        public bool Valid { get; private set; }

        public ZeeplevelFile()
        {
            GenerateBaseFile();
        }

        public ZeeplevelFile(string path)
        {
            GenerateBaseFile();
            ReadFromPath(path);
            if(!Valid)
            {
                Plugin.Instance.LogMessage("Invalid!");
            }
        }

        public ZeeplevelFile(string[] csvData)
        {
            GenerateBaseFile();
            ReadCSVData(csvData);
        }

        public void SetPlayerName(string playerName)
        {
            Header.GenerateNewUUID(playerName, Blocks.Count);
        }

        public void SetFileName(string fileName)
        {
            FileName = fileName;
        }

        public void SetPath(string path)
        {
            FilePath = path;
            FileName = Path.GetFileNameWithoutExtension(path);
        }

        public void ImportBlockProperties(List<BlockProperties> blockProperties)
        {
            Blocks.Clear();

            foreach (BlockProperties bp in blockProperties)
            {
                ZeeplevelBlock block = new ZeeplevelBlock();
                block.ReadBlockProperties(bp);
                if(block.Valid)
                {
                    Blocks.Add(block);
                }                
            }

            Header.GenerateNewUUID(Header.PlayerName, Blocks.Count);
        }

        private void GenerateBaseFile()
        {
            Header = new ZeeplevelHeader();
            Blocks = new List<ZeeplevelBlock>();
            FileName = "";
            FilePath = "";
            Valid = true;
        }

        private void ReadFromPath(string path)
        { 
            string[] csvData;

            try
            {
                csvData = File.ReadAllLines(path);
            }
            catch(Exception ex)
            {
                Plugin.Instance.LogMessage(ex.Message);
                Valid = false;
                return;
            }
            
            FileName = Path.GetFileNameWithoutExtension(path);
            FilePath = path;

            Valid = ReadCSVData(csvData);
        }        

        private bool ReadCSVData(string[] csvData)
        {
            if (csvData.Length < 3)
            {
                Plugin.Instance.LogMessage("CSV Data not valid.");
                return false;
            }

            // Read the first 3 lines into the header
            string[] headerData = new string[3];
            Array.Copy(csvData, 0, headerData, 0, 3);
            Header.ReadCSVData(headerData);

            if(!Header.Valid)
            {
                return false;
            }

            Blocks.Clear();
            // Read the remaining lines into blocks
            for (int i = 3; i < csvData.Length; i++)
            {
                ZeeplevelBlock block = new ZeeplevelBlock();
                block.ReadCSVString(csvData[i]);

                if(block.Valid)
                {
                    Blocks.Add(block);
                }                
            }

            return true;
        }

        public string[] ToCSV()
        {
            List<string> csvLines = new List<string>();

            // Add the header CSV lines
            csvLines.AddRange(Header.ToCSV());

            // Add each block's CSV representation
            foreach (var block in Blocks)
            {
                var blockCsv = block.ToCSV();
                if (!string.IsNullOrWhiteSpace(blockCsv))
                {
                    csvLines.Add(blockCsv);
                }
            }

            // Remove any empty lines
            csvLines = csvLines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

            return csvLines.ToArray();
        }
    }
}
