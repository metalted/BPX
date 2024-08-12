using System.Threading.Tasks;
using BPX.Api.Request;
using BPX.Api.Response;
using RestEase;

namespace BPX.Api
{

    public interface IAuthApi
    {
        [Post("auth/login")]
        Task<Response<AuthenticationResponseData>> Login([Body] AuthLoginRequestData data);

        [Post("auth/refresh")]
        Task<Response<AuthenticationResponseData>> Refresh([Body] AuthRefreshRequestData data);
    }
}
