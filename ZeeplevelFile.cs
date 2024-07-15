using System;
using System.Collections.Generic;
using System.Text;

namespace BPX
{
    public class ZeeplevelFile
    {
        public ZeeplevelHeader Header { get; private set; }
        public List<ZeeplevelBlock> Blocks { get; private set; }
        public string FileName { get; private set; }

        // Default constructor: creates a valid file with header and 0 blocks
        public ZeeplevelFile(string fileName)
        {
            Header = new ZeeplevelHeader();
            Blocks = new List<ZeeplevelBlock>();
            FileName = fileName;
        }

        // Constructor that initializes from CSV data
        public ZeeplevelFile(string[] csvData, string fileName)
        {
            if (csvData.Length < 3)
            {
                Header = new ZeeplevelHeader();
                Blocks = new List<ZeeplevelBlock>();
                FileName = fileName;
                return;
            }

            // Read the first 3 lines into the header
            string[] headerData = new string[3];
            Array.Copy(csvData, 0, headerData, 0, 3);
            Header = new ZeeplevelHeader(headerData);

            // Read the remaining lines into blocks
            Blocks = new List<ZeeplevelBlock>();
            for (int i = 3; i < csvData.Length; i++)
            {
                Blocks.Add(new ZeeplevelBlock(csvData[i]));
            }

            FileName = fileName;
        }

        // Constructor that initializes from a list of blocks, player name, and file name
        public ZeeplevelFile(List<ZeeplevelBlock> blocks, string playerName, string fileName)
        {
            Header = new ZeeplevelHeader();
            Header.GenerateNewUUID(playerName, blocks.Count);
            Blocks = new List<ZeeplevelBlock>(blocks);
            FileName = fileName;
        }

        // Converts the entire ZeeplevelFile to a CSV string
        public string ToCSV()
        {
            StringBuilder csvBuilder = new StringBuilder();

            // Add the header CSV
            csvBuilder.Append(Header.ToCSV());

            // Add each block's CSV representation
            foreach (var block in Blocks)
            {
                csvBuilder.AppendLine(block.ToCSV());
            }

            return csvBuilder.ToString();
        }
    }
}
