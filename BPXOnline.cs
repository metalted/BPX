using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BPX.Api.Response;
using UnityEngine;
using UnityEngine.Events;
using ZeepSDK.External.Cysharp.Threading.Tasks;

namespace BPX
{
    public static class BPXOnline
    {
        private static BPXOnlineUploadFile fileToUpload;

        public static void SetFileToUpload(BPXOnlineUploadFile file)
        {
            fileToUpload = file;
        }

        public static async UniTaskVoid CheckForOverwrite(UnityAction<bool> callback)
        {
            if (fileToUpload == null)
            {
                return;
            }

            bool exists = await BPXApi.Exists(fileToUpload.name);
            callback(exists);
        }

        public static async UniTask Upload()
        {
            if (fileToUpload == null)
            {
                return;
            }

            string blueprintBase64 = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, fileToUpload.file.ToCSV())));
            string imageBase64 = Convert.ToBase64String(fileToUpload.thumbnail.EncodeToPNG());
            await BPXApi.Submit(fileToUpload.name, fileToUpload.tags, blueprintBase64, imageBase64);
            Plugin.Instance.LogScreenMessage("BPXOnline: Uploading Complete!");
        }

        public static void SearchQuery(string query, UnityAction<List<BPXOnlineSearchResult>> callback)
        {
            if (string.IsNullOrEmpty(query.Trim()))
            {
                callback(new List<BPXOnlineSearchResult>());
            }
            else
            {
                Search(new BPXOnlineSearchQuery(query), callback).Forget();
            }
        }

        private static async UniTaskVoid Search(BPXOnlineSearchQuery query, UnityAction<List<BPXOnlineSearchResult>> callback)
        {
            List<BlueprintData> blueprintDatas = await BPXApi.Search(query.creator, query.tags, query.searchTerms);
            List<BPXOnlineSearchResult> results = new();

            foreach (BlueprintData blueprintData in blueprintDatas)
            {
                results.Add(
                    new BPXOnlineSearchResult()
                    {
                        creator = blueprintData.User.SteamName,
                        name = blueprintData.Name,
                        path = blueprintData.FileId,
                        steamID = blueprintData.IdUser,
                        thumbnail = null // TODO: This should probably be the url to the thumbnail
                    });
            }

            callback(results);
        }

        public static async UniTaskVoid DownloadSearchResultTo(BPXOnlineSearchResult result, string path, UnityAction callback)
        {
            Plugin.Instance.LogScreenMessage("Starting download...");

            string blueprintContents = await BPXApi.DownloadBlueprint((int)result.steamID, result.path);
            if (string.IsNullOrEmpty(blueprintContents))
            {
                Plugin.Instance.LogScreenMessage("Something went wrong :S");
                return;
            }

            string tempFileName = Path.GetTempFileName();
            await File.WriteAllTextAsync(tempFileName, blueprintContents);

            ZeeplevelFile file = ZeeplevelHandler.LoadFromFile(tempFileName);
            if (file.Valid)
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
