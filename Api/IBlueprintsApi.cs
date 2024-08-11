using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BPX.Api.Request;
using BPX.Api.Response;
using RestEase;

namespace BPX.Api;

public interface IBlueprintsApi
{
    [Get("blueprints/download/blueprint")]
    Task<HttpResponseMessage> Download(
        [Header("Authorization")] string authorization,
        [Query] int userId,
        [Query] string fileId);
    
    [Get("blueprints/download/image")]
    Task<HttpResponseMessage> DownloadImage(
        [Header("Authorization")] string authorization,
        [Query] int userId,
        [Query] string fileId);

    [Get("blueprints/exists")]
    Task<Response<bool>> Exists(
        [Header("Authorization")] string authorization,
        [Query] string name);

    [Post("blueprints/search")]
    Task<Response<List<BlueprintData>>> Search(
        [Header("Authorization")] string authorization,
        [Body] BlueprintSearchRequestData data);

    [Post("blueprints/submit")]
    Task<Response<BlueprintData>> Submit(
        [Header("Authorization")] string authorization,
        [Body] BlueprintSubmitRequestData data);
}
