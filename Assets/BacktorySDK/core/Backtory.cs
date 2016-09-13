using Assets.BacktorySDK.auth;
using RestSharp;
using RestSharp.Deserializers;
using System;
using UnityEngine;

namespace Assets.BacktorySDK.core
{
    public class Backtory
    {
        public const string BacktoryBaseAddress = "https://api.backtory.com/"; //"http://192.168.0.100:8043/"
        internal const string ContentTypeString = "Content-Type";
        internal const string ApplicationJson = "application/json";
        internal const string TextPlain = "text/plain";
        internal const string AuthInstanceIdString = "X-Backtory-Authentication-Id";
        internal const string AuthClientKeyString = "X-Backtory-Authentication-Key";
        internal const string GameInstanceIdString = "X-Backtory-Game-Id";
        //internal const string CloudCodeInstanceIdString = "";


        internal static readonly RestClient RestClient = new RestClient(BacktoryBaseAddress);

        //internal static readonly RestClient RestClient;
        public static IStorage Storage
        {
            internal get; set;
        }
        

        //public static Newtonsoft.Json.JsonSerializer JsonDonNetInstance { get; private set; } = new 

        static Backtory()
        {
            Storage = new FileStorage();//new PlayerPrefsStorage();

            //using NewtonSoft Json.Net
            //RestClient = new RestClient(BacktoryBaseAddress);
            //var JsonDotNetHandler = new NewtonsoftJsonSerializer();
            //RestClient.AddHandler("application/json", JsonDotNetHandler);
            //RestClient.AddHandler("text/json", JsonDotNetHandler);
            //RestClient.AddHandler("text/x-json", JsonDotNetHandler);
            //RestClient.AddHandler("text/javascript", JsonDotNetHandler);
            //RestClient.AddHandler("*+json", JsonDotNetHandler);
        }


        // this is absolutely terrible! some facts
        // * SimpleJson doesn't care about any serialize attribute (annotation in Java) so serialization faces with problem
        // * SimpleJson is an internal class in restsharp assembly so I doesn't have access and cat's set serializationStrategy
        // * Json.Net port for unity is big in size (450 kB) and have issues in versioning
        // * This one makes me miss Retrofit so much! I can't set global serializer so I must to attach my own serializer to every *** request 
        internal static IRestRequest RestRequest(string segmentUrl, Method method)
        {
            return new RestRequest(segmentUrl, method)
            {
                JsonSerializer = new NewtonsoftJsonSerializer() /*MyJsonSerializer();*/
            };
        }

        internal static BacktoryResponse<T> ExecuteRequest<T>(IRestRequest request) where T : class
        {
            var response = RestClient.Execute(request);
            BacktoryResponse<T> result = RawResponseToBacktoryResponse<T>(response);
            return result;
        }

        //internal static BacktoryResponse<T> ExecuteRequest<T>(RestRequest request) where T : class, new()
        //{
        //    var response = RestClient.Execute<T>(request);
        //    BacktoryResponse<T> result = RawResponseToBacktoryResponse(response);
        //    return result;
        //}

        internal static void ExecuteRequestAsync<T>(IRestRequest request, Action<BacktoryResponse<T>> callback) where T : class
        {
            RestClient.ExecuteAsync(request, response =>
            {
                // will be executed in background thread
                BacktoryResponse<T> result = RawResponseToBacktoryResponse<T>(response);

                // avoiding NullReferenceException on requests with null callback like logout
                if (callback != null)
                    BacktoryManager.Instance.Invoke(() => callback(result));
            });
        }

        private static BacktoryResponse<T> RawResponseToBacktoryResponse<T>(IRestResponse response) where T : class
        {
            BacktoryResponse<T> result;
            if (response.ErrorException != null || !response.IsSuccessful())
            {
                if ((int)response.StatusCode == (int)BacktoryHttpStatusCode.Unauthorized)
                    response = Handle401StatusCode(response);
                var errorResponse = TryToReadErrorBody(response);
                string message = errorResponse != null ? errorResponse.ErrorDescription : null;
                result = BacktoryResponse<T>.Error((int)response.StatusCode, message ?? response.ErrorMessage);
            }
            else
            {
                T res = response.Content as T; // try to cast content to T. Only true when T has defined as string.
                // if res is not null, it is raw response string which we need when response type has defined as string.
                // if is null, we can deserialize the content to T
                result = BacktoryResponse<T>.Success((int)response.StatusCode, res ?? FromJson<T>(response.Content));
            }
            Debug.Log("Receiving response of: " + typeof(T).Name + " with code: " + result.Code + "\ncause: " + response.ErrorMessage);
            Debug.Log(response.ResponseUri);
            return result;
        }

        private static ErrorResponse TryToReadErrorBody(IRestResponse response)
        {
            ErrorResponse error = null;
            try
            {
                error = FromJson<ErrorResponse>(response.Content);
            }
            catch (Exception)
            {
                Debug.Log("couldn't read error response: " + response.Content);
            }
            return error;
        }

        //private static BacktoryResponse<T> RawResponseToBacktoryResponse<T>(IRestResponse<T> response) where T : class, new()
        //{
        //    BacktoryResponse<T> result;
        //    if (response.ErrorException != null || !response.IsSuccessful())
        //    {
        //        if ((int)response.StatusCode == (int)BacktoryHttpStatusCode.Unauthorized)
        //            response = Handle401StatusCode(response);
        //        result = BacktoryResponse<T>.Error((int)response.StatusCode, response.ErrorMessage);
        //    }
        //    else
        //        result = BacktoryResponse<T>.Success((int)response.StatusCode, response.Data);
        //    Debug.Log("Receiving response of: " + typeof(T).Name + " with code: " + result.Code + "\ncause: " + response.ErrorMessage);
        //    Debug.Log(response.ResponseUri);
        //    return result;
        //}

        /// <summary>
        /// A 401 error mostly indicates access-token is expired. (Only exception is login which 401 shows incorrect username/password)
        /// We must refresh the access-token using refresh-token and retry the original request with new access-token
        /// If on refreshing access-token we get another 401, it indicates refresh-token is expired, too
        /// On that case, if current user is guest we must login with stored username-pass and if not we must force the user to login 
        /// </summary>
        /// <typeparam name="T">type of response body</typeparam>
        /// <param name="response">raw response containing error 401</param>
        /// <returns></returns>
        private static IRestResponse Handle401StatusCode(IRestResponse response) 
        {
            // in response of login request (no access token yet!) return the original response
            if (response.Request.Resource.Contains("login"))
                return response;

            // In some request like cloudcode's run, 401 might indicate to absence of access-token not the expiration.
            // We detect this by checking access-token nullity and simply just return the response.  
            if (BacktoryUser.GetAccessToken() == null)
                return response;
            // getting new access-token
            var tokenResponse = RestClient.Execute<BacktoryUser.LoginResponse>(BacktoryUser.NewAccessTokenRequest());

            if (tokenResponse.ErrorException != null || !response.IsSuccessful())
            {
                // failed to get new token
                if ((int)tokenResponse.StatusCode == (int)BacktoryHttpStatusCode.Unauthorized) // 401
                {
                    // refresh token itself is expired
                    if (BacktoryUser.GetCurrentUser().Guest)
                    {
                        // if guest, first login with stored username/pass and the retry the request
                        // new token is stored and after this we can simply call original request again which uses new token 
                        BacktoryUser.Login(BacktoryUser.GetCurrentUser().Username, BacktoryUser.GetGuestPassword());
                    }

                    // normal user must login again
                    // TODO: clean way for forcing user to login. How to keep his/her progress? How to retry original request?
                    else
                    {
                        BacktoryUser.ClearBacktoryStoredData();

                        // On this case return value is not important
                        // TODO: may be changing the response error message
                        BacktoryManager.Instance.GlobalEventListener.OnEvent(BacktorySDKEvent.LogoutEvent());
                    }
                }

                // successfully gotten new token
            }
            return RestClient.Execute(response.Request);
        }

        

        #region json converting
        internal static T FromJson<T>(string jsonString)
        {
            // in case of empty response return null
            if (typeof(T) == typeof(object) || jsonString.IsEmpty())
                return default(T);
            // I'm forced to create a dummy restresponse to make the deserializer work! 
            // Because it doesn't have a method getting string as parameter.
            return new JsonDeserializer().Deserialize<T>(
                            new RestResponse
                            {
                                Content = jsonString
                            });
        }

        // Not tested :)
        internal static object FromJson(string jsonString, Type t)
        {
            //return JsonConvert.DeserializeObject(jsonString, t);
            return new NewtonsoftJsonSerializer().Deserialize(jsonString, t);
        }

        internal static string ToJson(object obj, bool pretty = false)
        {
            var jsonSerializer = new NewtonsoftJsonSerializer(pretty);
            return jsonSerializer.Serialize(obj);
            //var JsonNet = new Newtonsoft.Json.JsonSerializer();
            //if (pretty)
            //    JsonNet.Formatting = Newtonsoft.Json.Formatting.Indented;
            //using (var stringWriter = new StringWriter())
            //{
            //    using (var jsonTextWriter = new JsonTextWriter(stringWriter))
            //    {
            //        JsonNet.Serialize(jsonTextWriter, obj);

            //        return stringWriter.ToString();
            //    }
            //}
        }
        #endregion

        internal static void Dispatch(Action action)
        {
            BacktoryManager.Instance.Invoke(action);
        }
        //internal static BacktoryConfig Config { get; private set; }

        //internal static void init(Config config)
        //{
        //    //Core.init(context);
        //    Config = config;
        //    BacktoryAuth.SetupAuth(config.BacktoryAuthInstanceId);
        //    //BacktoryCloudCode.setupCloudCode(config.BacktoryCloudcodeInstanceId);
        //    //BacktoryGame.setupGame(config.BacktoryGameInstanceId);
        //}
    }

    #region rest response extensions
    internal static class restSharpResponseSuccessfulExtension
    {
        internal static bool IsSuccessful(this IRestResponse restResponse)
        {
            return (int)restResponse.StatusCode < 300 &&
                (int)restResponse.StatusCode >= 200 &&
                restResponse.ResponseStatus == ResponseStatus.Completed;
        }
    }
    #endregion
}
