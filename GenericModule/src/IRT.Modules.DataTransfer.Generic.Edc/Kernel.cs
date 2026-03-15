using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace IRT.Modules.DataTransfer.Generic.Edc
{
    public static class Kernel
    {
        /// <summary>
        /// Extension to create a HttpResponseMessage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="statusCode"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static HttpResponseMessage CreateResponse<T>(
            this HttpRequest request, HttpStatusCode statusCode, T content)
        {
            return new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.Serialize(content))
            };
        }

        /// <summary>
        /// Extension to create a HttpResponseMessage
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statusCode"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static HttpResponseMessage CreateResponse(
            this HttpRequest request, HttpStatusCode statusCode, StringContent content)
        {
            return new HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = content
            };
        }

        /// <summary>
        /// Extension to create a HttpResponseMessage
        /// </summary>
        /// <returns></returns>
        public static HttpResponseMessage CreateResponse(this HttpRequest request)
        {
            return new HttpResponseMessage();
        }
    }
}
