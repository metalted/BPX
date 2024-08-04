using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace BPX
{
    public static class BPXOnline
    {
        private static BPXOnlineUploadFile fileToUpload;

        public static void SetFileToUpload(BPXOnlineUploadFile file)
        {
            fileToUpload = file;
        }

        public static void CheckForOverwrite(UnityAction<bool> callback)
        {
            if(fileToUpload == null) { return; }

            //Do server stuff...

            bool isOverwrite = IsOverwrite(fileToUpload.creator, fileToUpload.name);
            callback(isOverwrite);
        }

        public static bool IsSetup()
        {
            return Directory.Exists(BPXConfiguration.GetBPXOnlineTestingDirectory());
        }
        
        public static bool IsOverwrite(string creator, string name)
        {
            if(!Directory.Exists(Path.Combine(BPXConfiguration.GetBPXOnlineTestingDirectory(), creator)))
            {
                return false;
            }

            string saveName = name.Replace(".zeeplevel", "").Trim();

            if (File.Exists(Path.Combine(BPXConfiguration.GetBPXOnlineTestingDirectory(), creator, saveName + ".zeeplevel")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Upload()
        {
            if(fileToUpload == null)
            {
                return;
            }

            // Create a folder with the name being the timestamp
            string folderPath = Path.Combine(BPXConfiguration.GetBPXOnlineTestingDirectory(), fileToUpload.creator);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Define the path for the file to upload
            string filePath = Path.Combine(folderPath, fileToUpload.name + ".zeeplevel");
            ZeeplevelHandler.SaveToFile(fileToUpload.file, filePath);

            // Save the image as a PNG
            string imageFileName = fileToUpload.name + "_Thumb.png";
            string imagePath = Path.Combine(folderPath, imageFileName);

            // Encode the image to PNG format
            byte[] pngData = fileToUpload.thumbnail.EncodeToPNG();

            // Write the PNG file
            File.WriteAllBytes(imagePath, pngData);

            Plugin.Instance.LogScreenMessage("BPXOnline: Uploading Complete!");
        }
        
        public static void SearchQuery(string query, UnityAction <List<BPXOnlineSearchResult>> callback)
        {
            if (!BPXOnline.IsSetup())
            {
                callback(new List<BPXOnlineSearchResult>());
                return;
            }

            if (string.IsNullOrEmpty(query.Trim()))
            {
                callback(new List<BPXOnlineSearchResult>());
                return;
            }
            else
            {
                Search(new BPXOnlineSearchQuery(query), callback);
            }
        }

        private static void Search(BPXOnlineSearchQuery query, UnityAction<List<BPXOnlineSearchResult>> callback)
        {
            string baseDirectory = BPXConfiguration.GetBPXOnlineTestingDirectory();

            // Determine the directory to search
            string searchDirectory = string.IsNullOrEmpty(query.creator) ? baseDirectory : Path.Combine(baseDirectory, query.creator);

            List<BPXOnlineSearchResult> results = new List<BPXOnlineSearchResult>();

            if (!Directory.Exists(searchDirectory))
            {
                callback(results);
                return;
            }

            // Get all .zeeplevel files in the directory and subdirectories
            var zeeplevelFiles = Directory.GetFiles(searchDirectory, "*.zeeplevel", SearchOption.AllDirectories);

            foreach (var filePath in zeeplevelFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                // Check if the file name contains all search terms
                bool containsAllSearchTerms = query.searchTerms.All(term => fileName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);

                if (containsAllSearchTerms)
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
                            creator = string.IsNullOrEmpty(query.creator) ? "The Player" : query.creator,
                            thumbnail = thumbnail
                        };

                        results.Add(result);
                    }
                }
            }

            callback(results);
        }

        public static void DownloadSearchResultTo(BPXOnlineSearchResult result, string path, UnityAction callback)
        {
            ZeeplevelFile file = ZeeplevelHandler.LoadFromFile(result.path);
            if(file.Valid)
            {
                ZeeplevelHandler.SaveToFile(file, path);
                callback();
            }   
            else
            {
                Plugin.Instance.LogScreenMessage("Something went wrong :S");
            }
        }
    }
}
