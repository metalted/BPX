using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace BPX
{
    public class BPXOnlineSearchResult
    {
        public string name;
        public string path;
        public string creator;
        public Texture2D thumbnail;
    }

    public static class BPXOnline
    {
        public static void Upload(ZeeplevelFile zeeplevel, Texture2D thumbnail, string creator, string name)
        {
            if (!Directory.Exists(BPXConfiguration.GetBPXOnlineTestingDirectory()))
            {
                Plugin.Instance.LogScreenMessage("BPXOnline Testing Directory doesn't exist!");
                return;
            }

            string saveName = name.Replace(".zeeplevel", "").Trim();
            if(string.IsNullOrEmpty(saveName))
            {
                Plugin.Instance.LogScreenMessage("Name cannot be empty");
                return;
            }

            // Create a timestamp
            string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");

            // Create a folder with the name being the timestamp
            string folderPath = Path.Combine(BPXConfiguration.GetBPXOnlineTestingDirectory(), timestamp);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Define the path for the file to upload
            string filePath = Path.Combine(folderPath, saveName + ".zeeplevel");
            ZeeplevelHandler.SaveToFile(zeeplevel, filePath);

            // Save the image as a PNG
            string imageFileName = saveName + "_Thumb.png";
            string imagePath = Path.Combine(folderPath, imageFileName);

            // Encode the image to PNG format
            byte[] pngData = thumbnail.EncodeToPNG();

            // Write the PNG file
            File.WriteAllBytes(imagePath, pngData);

            Plugin.Instance.LogScreenMessage("BPXOnline: Uploading Complete!");
        }

        public static void Search(string query)
        {
            string baseDirectory = BPXConfiguration.GetBPXOnlineTestingDirectory();

            if (!Directory.Exists(baseDirectory))
            {
                Plugin.Instance.LogScreenMessage("BPXOnline Testing Directory doesn't exist!");
                return;
            }

            Plugin.Instance.LogScreenMessage("Searching for " + query + "...");

            // Get all .zeeplevel files in the directory and subdirectories
            var zeeplevelFiles = Directory.GetFiles(baseDirectory, "*.zeeplevel", SearchOption.AllDirectories);

            List<BPXOnlineSearchResult> results = new List<BPXOnlineSearchResult>();

            foreach (var filePath in zeeplevelFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                // Check if the file name contains the query
                if (fileName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Check for corresponding thumbnail
                    string thumbnailPath = Path.Combine(Path.GetDirectoryName(filePath), fileName + "_Thumb.png");

                    if (File.Exists(thumbnailPath))
                    {
                        // Read the thumbnail image into a Texture2D
                        byte[] thumbnailData = File.ReadAllBytes(thumbnailPath);
                        Texture2D thumbnail = new Texture2D(2, 2);
                        thumbnail.LoadImage(thumbnailData);

                        // Create and populate the search result
                        BPXOnlineSearchResult result = new BPXOnlineSearchResult
                        {
                            name = fileName,
                            path = filePath,
                            creator = "Player",
                            thumbnail = thumbnail
                        };

                        results.Add(result);
                    }
                }
            }

            // You can handle the results list as needed
            if (results.Count > 0)
            {
                Plugin.Instance.LogScreenMessage($"Found {results.Count} results for '{query}'");                
            }
            else
            {
                Plugin.Instance.LogScreenMessage("No results found for " + query);
            }

            BPXUIManagement.OnOnlineSearchResults(results);
        }
    }
}
