using RestSharp;
using System.Net;

namespace RatesApiTests.TestData
{
    public static class ResponseData
    {
        public static IRestResponse<T> GetSuccesResponse<T>(T responseData)
        {
            return new RestResponse<T>
            {
                Data = responseData,
                StatusCode = HttpStatusCode.OK
            };
        }

        public static IRestResponse<T> GetErrorResponse<T>()
        {
            return new RestResponse<T>
            {
                ErrorMessage = Constants._error,
                StatusCode = default
            };
        }
        public static IRestResponse<T> GetBadResponse<T>()
        {
            return new RestResponse<T>
            {
                Content = Constants._content,
                StatusCode = HttpStatusCode.Forbidden
            };
        }
    }
}
