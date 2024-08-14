using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using BPX.Api;
using BPX.Api.Request;
using BPX.Api.Response;
using RestEase;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using ZeepSDK.External.Cysharp.Threading.Tasks;

namespace BPX
{

    public static class BPXApi
    {
        private static AuthenticationResponseData authenticationData;

        /// <summary>
        /// This will automatically login the current steam user to your backend
        /// </summary>
        public static async UniTask Login()
        {
            IAuthApi authApi = RestClient.For<IAuthApi>(BPXConfiguration.GetBPXOnlineApiUrl());
            Response<AuthenticationResponseData> response = await authApi.Login(
                new AuthLoginRequestData()
                {
                    AuthenticationTicket = CreateAuthenticationTicket(),
                    SteamId = SteamClient.SteamId,
                    SteamName = SteamClient.Name
                });

            try
            {
                response.ResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                authenticationData = null;
                Debug.LogError("Failed to login");
                Debug.LogException(e);
                return;
            }

            authenticationData = response.GetContent();
        }

        /// <summary>
        /// This will automatically refresh the current steam user's authentication token.
        /// The authentication token is automatically invalidated after 5 minutes.
        /// </summary>
        public static async UniTask Refresh()
        {
            IAuthApi authApi = RestClient.For<IAuthApi>(BPXConfiguration.GetBPXOnlineApiUrl());
            Response<AuthenticationResponseData> response = await authApi.Refresh(
                new AuthRefreshRequestData()
                {
                    LoginToken = authenticationData.AccessToken,
                    RefreshToken = authenticationData.RefreshToken,
                    SteamId = SteamClient.SteamId
                });

            try
            {
                response.ResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to refresh authentication");
                Debug.LogException(e);
                return;
            }

            authenticationData = response.GetContent();
        }

        private static string CreateAuthenticationTicket()
        {
            AuthTicket authSessionTicket = SteamUser.GetAuthSessionTicket(new NetIdentity());
            StringBuilder stringBuilder = new();
            foreach (byte b in authSessionTicket.Data)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }

            return stringBuilder.ToString();
        }

        private static async UniTask RefreshIfExpired()
        {
            if (authenticationData == null || long.Parse(authenticationData.RefreshTokenExpiry) < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                await Login();
                return;
            }

            if (long.Parse(authenticationData.AccessTokenExpiry) >= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                return;
            }

            try
            {
                await Refresh();
                return;
            }
            catch (Exception e)
            {
                // Refreshing failed so we're just gonna try to login normally
                // Usually you would want to log this to figure out what is going on etc
                Debug.LogException(e);
                authenticationData = null;
            }

            await Login();
        }

        /// <summary>
        /// This can be used to check if a blueprint with the given name exists for the current user
        /// </summary>
        public static async UniTask<bool> Exists(string name)
        {
            await RefreshIfExpired();
            IBlueprintsApi blueprintsApi = RestClient.For<IBlueprintsApi>(BPXConfiguration.GetBPXOnlineApiUrl());
            Response<bool> response = await blueprintsApi.Exists("Bearer " + authenticationData.AccessToken, name);

            try
            {
                response.ResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to search blueprints");
                Debug.LogException(e);
                return false; // TODO: You have to decide what you want to return in this case
            }

            return response.GetContent();
        }

        public static async UniTask<List<BlueprintData>> Latest(int amount)
        {
            IBlueprintsApi blueprintsApi = RestClient.For<IBlueprintsApi>(BPXConfiguration.GetBPXOnlineApiUrl());
            Response<List<BlueprintData>> response = await blueprintsApi.Latest(amount);

            try
            {
                response.ResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to search blueprints");
                Debug.LogException(e);
                return new List<BlueprintData>();
            }

            return response.GetContent();
        }

        /// <summary>
        /// This can be used to search for blueprints with the given creator, tags, and terms
        /// </summary>
        public static async UniTask<List<BlueprintData>> Search(
            string creator,
            IEnumerable<string> tags,
            IEnumerable<string> terms)
        {
            await RefreshIfExpired();
            IBlueprintsApi blueprintsApi = RestClient.For<IBlueprintsApi>(BPXConfiguration.GetBPXOnlineApiUrl());
            Response<List<BlueprintData>> response = await blueprintsApi.Search(
                "Bearer " + authenticationData.AccessToken,
                new BlueprintSearchRequestData()
                {
                    Creator = creator,
                    Tags = tags.ToList(),
                    Terms = terms.ToList()
                });

            try
            {
                response.ResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to search blueprints");
                Debug.LogException(e);
                return new List<BlueprintData>();
            }

            return response.GetContent();
        }

        /// <summary>
        /// Use this to submit a blueprint to the BPX online API
        /// </summary>
        public static async UniTask Submit(
            string name,
            IEnumerable<string> tags,
            string blueprintBase64,
            string imageBase64)
        {
            await RefreshIfExpired();
            IBlueprintsApi blueprintsApi = RestClient.For<IBlueprintsApi>(BPXConfiguration.GetBPXOnlineApiUrl());
            Response<BlueprintData> response = await blueprintsApi.Submit(
                "Bearer " + authenticationData.AccessToken,
                new BlueprintSubmitRequestData()
                {
                    Name = name,
                    Tags = tags.ToList(),
                    BlueprintBase64 = blueprintBase64,
                    ImageBase64 = imageBase64
                });

            try
            {
                response.ResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to submit blueprint");
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// This downloads the blueprint as a string (zeeplevel)
        /// </summary>
        public static async UniTask<string> DownloadBlueprint(int userId, string fileId)
        {
            await RefreshIfExpired();
            IBlueprintsApi blueprintsApi = RestClient.For<IBlueprintsApi>(BPXConfiguration.GetBPXOnlineApiUrl());
            HttpResponseMessage response = await blueprintsApi.Download(
                "Bearer " + authenticationData.AccessToken,
                userId,
                fileId);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to download blueprint");
                Debug.LogException(e);
                return string.Empty;
            }

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// This downloads the blueprint as a byte array (png)
        /// </summary>
        public static async UniTask<byte[]> DownloadImage(int userId, string fileId)
        {
            await RefreshIfExpired();
            IBlueprintsApi blueprintsApi = RestClient.For<IBlueprintsApi>(BPXConfiguration.GetBPXOnlineApiUrl());
            HttpResponseMessage response = await blueprintsApi.DownloadImage(
                "Bearer " + authenticationData.AccessToken,
                userId,
                fileId);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to download image");
                Debug.LogException(e);
                return Array.Empty<byte>();
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
