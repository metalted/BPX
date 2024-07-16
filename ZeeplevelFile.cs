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
        public ZeeplevelFile()
        {
            Header = new ZeeplevelHeader();
            Blocks = new List<ZeeplevelBlock>();
            FileName = "";
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

        // Constructor that initializes from a list of blocks, player name, and file name
        public ZeeplevelFile(List<ZeeplevelBlock> blocks)
        {
            Header = new ZeeplevelHeader();
            Header.GenerateNewUUID("Bouwerman", blocks.Count);
            Blocks = new List<ZeeplevelBlock>(blocks);
            FileName = "";
        }

        public string[] ToCSV()
        {
            List<string> csvLines = new List<string>();

            // Add the header CSV lines
            csvLines.AddRange(Header.ToCSV().Split(new[] { Environment.NewLine }, StringSplitOptions.None));

            // Add each block's CSV representation
            foreach (var block in Blocks)
            {
                csvLines.Add(block.ToCSV());
            }

            return csvLines.ToArray();
        }
    }
}
