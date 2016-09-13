using Assets.BacktorySDK.auth;
using Assets.BacktorySDK.core;
using RestSharp;
using System;

namespace Assets.BacktorySDK.cloudcode
{
    class BacktoryCloudcode
    {
        private static BacktoryCloudcode instance = new BacktoryCloudcode();
        // all methods could be static but we get a threading exception from PlayerPrefs
        /* GetString can only be called from the main thread.
        Constructors and field initializers will be executed from the loading thread when loading a scene.
        Don't use this function in the constructor or field initializers, instead move initialization code to the Awake or Start function. */

        private IRestRequest CloudCodeRequest(string functionName, object requestBody, Type responseType)
        {
            var request = Backtory.RestRequest("cloud-code/{cloud_instance_id}/{function_name}", Method.POST);
            request.Timeout = 20 * 1000; // 20 seconds
            request.AddParameter("cloud_instance_id", BacktoryConfig.BacktoryCloudcodeInstanceId, ParameterType.UrlSegment);
            request.AddParameter("function_name", functionName, ParameterType.UrlSegment);
            request.AddHeader(BacktoryUser.KeyAuthorization, BacktoryUser.AuthorizationHeader());

            if (requestBody.GetType() != typeof(string))
            {
                request.AddJsonBody(requestBody);
                request.OnBeforeDeserialization = irestresponse => irestresponse.ContentType = Backtory.ApplicationJson;
            }
            else
            {
                request.AddParameter(Backtory.TextPlain, requestBody, ParameterType.RequestBody);
                request.OnBeforeDeserialization = irestresponse => irestresponse.ContentType = Backtory.TextPlain;
            }
            return request;
        }


        /// <summary>
        /// Synchronously sends the argument for backtory cloud function specified with <c>functionName</c>
        /// waits for execution and returns its response.
        /// </summary>
        /// <typeparam name="T"> Type of expected response.</typeparam>
        /// <param name="functionName">name of cloud function you've set in backtory panel.</param>
        /// <param name="requestBody">input argument for cloud function.If a String object, will be send intact if anything else will be serialized to json.</param>
        /// <returns>result of cloud function execution wrapped in a <see cref="BacktoryResponse{T}"/></returns>
        public static BacktoryResponse<T> Run<T>(string functionName, object requestBody) where T : class
        {
            return Backtory.ExecuteRequest<T>(instance.CloudCodeRequest(functionName, requestBody, typeof(T)));
        }


        /// <summary>
        /// Sends the argument for backtory cloud function specified with <c>functionName</c> in
        /// background and returns its execution response.
        /// </summary>
        /// <typeparam name="T"> Type of expected response.</typeparam>
        /// <param name="functionName">name of cloud function you've set in backtory panel.</param>
        /// <param name="requestBody">input argument for cloud function.If a String object, will be send intact if anything else will be serialized to json.</param>
        /// <param name="callback">callback notified upon receiving server response or any error in the
        /// process. Server response is the result of cloud function execution wrapped in a <see cref="BacktoryResponse{T}"/></param>
        public static void RunInBackground<T>(string functionName, object requestBody, Action<BacktoryResponse<T>> callback) where T : class
        {
            Backtory.ExecuteRequestAsync(instance.CloudCodeRequest(functionName, requestBody, typeof(T)), callback);
        }
    }
}
