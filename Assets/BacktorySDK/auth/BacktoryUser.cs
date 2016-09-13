using Assets.BacktorySDK.core;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.BacktorySDK.auth
{
    public class BacktoryUser
    {
        private static BacktoryUser currentUser;
        private const string KeyGuestPassword = "guest password";
        private const string KeyLoginInfo = "login info";
        internal const string KeyAuthorization = "Authorization";
        private const string KeyCurrentUser = "current user";
        private const string KeyUsername = "username key";


        #region private storing methods

        private static void SaveLoginInfo(LoginResponse loginResponse)
        {
            Backtory.Storage.Put(KeyLoginInfo, Backtory.ToJson(loginResponse));
        }
        private static void DispatchSaveLoginInfo(LoginResponse loginResponse)
        {
            Backtory.Dispatch(() => { SaveLoginInfo(loginResponse); });
        }

        private static void SaveGuestPassword(string guestPassword)
        {
            Backtory.Storage.Put(KeyGuestPassword, guestPassword);
        }
        private static void DispatchSaveGuestPassword(string guestNewPassword)
        {
            Backtory.Dispatch(() => { SaveGuestPassword(guestNewPassword); });
        }

        private static void SaveAsCurrentUserInMemoryAndStorage(BacktoryUser user)
        {
            currentUser = user;
            Backtory.Storage.Put(KeyUsername, user.Username);
            Backtory.Storage.Put(KeyCurrentUser, Backtory.ToJson(user));
        }
        private static void DispatchSaveCurrentUser(BacktoryUser backtoryUser)
        {
            Backtory.Dispatch(() => { SaveAsCurrentUserInMemoryAndStorage(backtoryUser); });
        }

        #endregion

        #region guest register + login
        private static IRestRequest GuestRegisterRequest()
        {
            var request = Backtory.RestRequest("auth/guest-users", Method.POST);
            request.AddHeader(Backtory.AuthInstanceIdString, BacktoryConfig.BacktoryAuthInstanceId);
            return request;
        }

        /// <summary>
        /// Registers a guest account on server and logins with that in background.
        /// <para>This is preferable to using <see cref="LoginAsGuest"/>, unless your code is already
        /// running from a background thread.</para>
        /// </summary>
        /// <param name="callback">callback notified upon receiving server response or any error in the
        /// process. Server response is login session details wrapped in a <c>BacktoryResponse</c>.</param>
        public static void LoginAsGuestInBackground(Action<BacktoryResponse<LoginResponse>> callback)
        {
            // Simplifying this by removing "BacktoryUser" type parameter leads to error though compiler suggests, but why?!
            Backtory.RestClient.ExecuteAsync<BacktoryUser>(GuestRegisterRequest(), response =>
            {
                // **this part will be called on background thread! (Since we're using restsharp async method)**

                // if guest register failed don't proceed to login 
                if (!response.IsSuccessful())
                {
                    Backtory.Dispatch(() => BacktoryResponse<LoginResponse>.Unknown(response.ErrorMessage));
                    return;
                }

                var guest = response.Data;
                var loginResponse = Backtory.ExecuteRequest<LoginResponse>(LoginRequest(guest.Username, guest.Password));

                if (loginResponse.Successful)
                {
                    DispatchSaveCurrentUser(guest);
                    DispatchSaveGuestPassword(guest.Password);
                    DispatchSaveLoginInfo(loginResponse.Body);
                }

                Backtory.Dispatch(() => callback(loginResponse));
            });
        }



        /// <summary>
        /// Synchronously registers a guest account on server and logins with that.
        /// <para>Typically, you should use <see cref="LoginAsGuestInBackground(Action{BacktoryResponse{LoginResponse}})"></see>
        /// instead of this, unless you are managing your own threading.</para>
        /// <para>Data of guest user automatically stored on this request succession and you can access that with
        /// <see cref="GetCurrentUser"></see></para>
        /// </summary>
        /// <returns>Login session details wrapped in a <c>BacktoryResponse</c>.</returns>
        public static BacktoryResponse<LoginResponse> LoginAsGuest()
        {
            var regResponse = Backtory.ExecuteRequest<BacktoryUser>(GuestRegisterRequest());
            if (!regResponse.Successful)
            {
                return BacktoryResponse.Error<BacktoryUser, LoginResponse>(regResponse);
            }
            var guest = regResponse.Body;
            var loginResponse = Backtory.ExecuteRequest<LoginResponse>(LoginRequest(guest.Username, guest.Password));

            if (loginResponse.Successful)
            {
                DispatchSaveCurrentUser(guest);
                DispatchSaveGuestPassword(guest.Password);
                DispatchSaveLoginInfo(loginResponse.Body);
            }

            return loginResponse;
        }
        #endregion

        #region register
        private static IRestRequest RegisterRequest(BacktoryUser registrationParams)
        {
            var request = Backtory.RestRequest("auth/users", Method.POST);
            request.AddHeader(Backtory.AuthInstanceIdString, BacktoryConfig.BacktoryAuthInstanceId);
            request.AddJsonBody(registrationParams);
            return request;
        }

        /// <summary>
        /// Registers this user on Backtory server in background. This is separate from activation process
        /// but if activation not set in Backtory panel, user will be active after this method execution.
        /// <para>This is preferable to using <see cref="Register"/>, unless your code is already running from a
        /// background thread.</para>
        /// </summary>
        /// <param name="callback"></param>
        public void RegisterInBackground(Action<BacktoryResponse<BacktoryUser>> callback)
        {
            Backtory.ExecuteRequestAsync(RegisterRequest(this), callback);
        }

        /// <summary>
        /// Synchronously registers this user on backtory servers and returns its response.
        /// This is separate from activation process
        /// but if activation not set in Backtory panel, user will be active after this method execution.
        /// <para>Typically, you should <see cref="RegisterInBackground(Action{BacktoryResponse{BacktoryUser}})"/>use instead of this,
        /// unless you are managing your own threading.</para>
        /// </summary>
        /// <returns>Backtory user registered on server wrapped in a <c>BacktoryResponse</c></returns>
        public BacktoryResponse<BacktoryUser> Register()
        {
            return Backtory.ExecuteRequest<BacktoryUser>(RegisterRequest(this));
        }
        #endregion

        #region login
        // restsharp normally doesn't support multipart request without file
        // by setting AlwaysMultipart we force it to do so
        private static IRestRequest LoginRequest(string username, string password)
        {
            var loginRequest = Backtory.RestRequest("auth/login", Method.POST);
            loginRequest.AlwaysMultipartFormData = true;
            loginRequest.AddHeader(Backtory.AuthInstanceIdString, BacktoryConfig.BacktoryAuthInstanceId);
            loginRequest.AddHeader("X-Backtory-Authentication-Key", BacktoryConfig.BacktoryAuthClientKey);
            loginRequest.AddParameter("username", username, ParameterType.GetOrPost);
            loginRequest.AddParameter("password", password, ParameterType.GetOrPost);

            return loginRequest;
        }

        /// <summary>
        /// Synchronously logins the specified user by username and password and returns its response.
        /// <para>Typically, you should use <see cref="LoginInBackground(string, string, Action{BacktoryResponse{BacktoryUser.LoginResponse}})"/>
        /// instead of this, unless you are managing your own threading.</para>
        /// </summary>
        /// <param name="username">username of application user who wants to login into the app.</param>
        /// <param name="password">password of application user who wants to login into the app.</param>
        /// <returns>Login session details wrapped in a <c>BacktoryResponse</c>.</returns>
        /// <seealso cref="LoginResponse"/>
        public static BacktoryResponse<LoginResponse> Login(string username, string password)
        {
            var loginResponse = Backtory.ExecuteRequest<LoginResponse>(LoginRequest(username, password));
            if (loginResponse.Successful)
            {
                var userResponse = Backtory.ExecuteRequest<BacktoryUser>(UserByUsernameRequest(username, loginResponse.Body.AccessToken));
                if (userResponse.Successful)
                {
                    DispatchSaveCurrentUser(userResponse.Body);
                    DispatchSaveLoginInfo(loginResponse.Body);
                }
                else
                {
                    BacktoryResponse<LoginResponse>.Unknown(userResponse.Message);
                    Debug.Log("error getting user info by username\n" + userResponse.Message);
                }
            }
            return loginResponse;
        }

        /// <summary>
        /// Logins the specified user by username and password in background.
        /// <para>This is preferable to using <see cref="Login(string, string)"/>, unless your code is already
        /// running from a background thread.</para>
        /// </summary>
        /// <param name="username">username of application user who wants to login into the app.</param>
        /// <param name="password">password of application user who wants to login into the app.</param>
        /// <param name="callback">callback notified upon receiving server response or any error in the
        /// process.Server response is login session details wrapped in a <c>BacktoryResponse</c>.</param>
        /// <seealso cref="LoginResponse"/>
        public static void LoginInBackground(string username, string password,
            Action<BacktoryResponse<LoginResponse>> callback)
        {
            Backtory.ExecuteRequestAsync<LoginResponse>(LoginRequest(username, password), loginResopnse =>
            {
                // this will be called in main thread since we're using backtory API
                if (loginResopnse.Successful)
                {
                    Backtory.ExecuteRequestAsync<BacktoryUser>(UserByUsernameRequest(username, loginResopnse.Body.AccessToken), userResponse =>
                    {
                        // also on main thread
                        if (userResponse.Successful)
                        {
                            //DispatchSaveCurrentUser(userResponse.Body);
                            //DispatchSaveLoginInfo(loginResopnse.Body);
                            SaveAsCurrentUserInMemoryAndStorage(userResponse.Body);
                            SaveLoginInfo(loginResopnse.Body);
                            callback(loginResopnse);
                        }
                        else
                            callback(BacktoryResponse<LoginResponse>.Unknown(userResponse.Message));

                    });
                }
                else
                    callback(loginResopnse);
            });
        }
        #endregion

        #region new access token
        internal static IRestRequest NewAccessTokenRequest()
        {
            var request = Backtory.RestRequest("auth/login", Method.POST);
            request.AlwaysMultipartFormData = true;
            request.AddHeader(Backtory.AuthInstanceIdString, BacktoryConfig.BacktoryAuthInstanceId);
            request.AddHeader(Backtory.AuthClientKeyString, BacktoryConfig.BacktoryAuthClientKey);
            request.AddHeader("X-Backtory-Authentication-Refresh", "1");
            request.AddParameter("refresh_token", BacktoryUser.GetRefreshToken(), ParameterType.GetOrPost);
            return request;
        }

        /// <summary>
        /// This is for test.
        /// </summary>
        /// <returns></returns>
        internal static void NewAccessTokenInBackground(Action<BacktoryResponse<LoginResponse>> callback)
        {
            Backtory.ExecuteRequestAsync(NewAccessTokenRequest(), callback);
        }
        #endregion

        #region current user
        /// <summary>
        /// After a login request Backtory SDK holds the information of logged-in user in RAM and Storage
        /// to prevent the network request every time user info is needed.
        /// </summary>
        /// <returns>currently logged-in user synchronously from memory if data exist on RAM and from DISK if not present
        /// in RAM.</returns>
        public static BacktoryUser GetCurrentUser()
        {
            // from memory
            if (currentUser != null)
            {
                return currentUser;
            }
            // from storage (mostly disk)
            string userJson = Backtory.Storage.Get(KeyCurrentUser);
            if (!userJson.IsEmpty())
            {
                return Backtory.FromJson<BacktoryUser>(userJson);
            }
            // indicating a login is required because a user info must be exist in all conditions if user
            // access token is present in storage
            return null;
        }
        #endregion

        #region user by username
        internal static IRestRequest UserByUsernameRequest(string username, string accessToken = null)
        {
            var request = Backtory.RestRequest("auth/users/by-username/{username}", Method.GET);
            request.AddHeader(Backtory.AuthInstanceIdString, BacktoryConfig.BacktoryAuthInstanceId);
            request.AddHeader(KeyAuthorization, accessToken != null ? "Bearer " + accessToken : AuthorizationHeader());
            request.AddParameter("username", username, ParameterType.UrlSegment);

            return request;
        }

        // TODO change to getCurrentUser by access token
        internal static BacktoryResponse<BacktoryUser> GetUserByUsername(string username)
        {
            return Backtory.ExecuteRequest<BacktoryUser>(UserByUsernameRequest(username));
        }
        #endregion

        #region complete guest register
        private IRestRequest CompleteRegRequest(GuestCompletionParam guestRegistrationParam)
        {
            var request = Backtory.RestRequest("auth/guest-users/complete-registration", Method.POST);
            request.AddHeader(Backtory.AuthInstanceIdString, BacktoryConfig.BacktoryAuthInstanceId);
            request.AddHeader(KeyAuthorization, AuthorizationHeader());
            request.AddJsonBody(guestRegistrationParam);
            return request;
        }

        /// <summary>
        /// Converts a guest user to a normal one with passed information in background.
        /// <para>You should get the user object by <see cref="GetCurrentUser()"/> because it contains the
        /// information which backtory server needs to distinct the guest user from others.</para>
        /// <para>This is preferable to using <see cref="CompleteRegistration(GuestCompletionParam)"/>,
        /// unless your code is already running from a background thread</para>
        /// </summary>
        /// <param name="guestRegistrationParam">information required for converting guest user to normal user</param>
        /// <param name="callback">callback notified upon receiving server response or any error in the
        /// process. Server response is newly converted user wrapped in a <c>BacktoryResponse</c></param>
        public void CompleteRegistrationInBackgrond(GuestCompletionParam guestRegistrationParam, Action<BacktoryResponse<BacktoryUser>> callback)
        {
            Backtory.ExecuteRequestAsync<BacktoryUser>(CompleteRegRequest(guestRegistrationParam), completeRegResponse =>
            {
                if (completeRegResponse.Successful)
                    SaveAsCurrentUserInMemoryAndStorage(completeRegResponse.Body);
                callback(completeRegResponse);
            });
        }

        /// <summary>
        /// Synchronously converts a guest user to a normal one with passed information
        /// in backtory server and returns its response.
        /// <para>You should get the user object by <see cref="GetCurrentUser"/> because it encloses the
        /// information which backtory server needs to distinct the guest user from others.</para>
        /// </summary>
        /// <param name="guestRegistrationParam">information required for converting guest user to normal user</param>
        /// <returns>newly converted user wrapped in a <c>BacktoryResponse</c></returns>
        public BacktoryResponse<BacktoryUser> CompleteRegistration(GuestCompletionParam guestRegistrationParam)
        {
            var completeRegResponse = Backtory.ExecuteRequest<BacktoryUser>(CompleteRegRequest(guestRegistrationParam));
            if (completeRegResponse.Successful)
                DispatchSaveCurrentUser(completeRegResponse.Body);
            return completeRegResponse;
        }
        #endregion

        #region change password
        private IRestRequest ChangePassRequest(string oldPassword, string newPassword)
        {
            var request = Backtory.RestRequest("auth/change-password", Method.POST);
            request.AddHeader(Backtory.AuthInstanceIdString, BacktoryConfig.BacktoryAuthInstanceId);
            request.AddHeader(KeyAuthorization, AuthorizationHeader());

            // we don't want to restsharp treat the empty response as json and try to deserialize it.
            request.OnBeforeDeserialization = response =>
            {
                response.ContentType = "text/plain";
            };
            request.AddJsonBody(new Dictionary<string, string>()
            {
                // it's common to refer as "oldPassword" not "lastPassword". But what can I do? :)
                { "lastPassword", oldPassword }, { "newPassword", newPassword }
            });

            return request;
        }

        /// <summary>
        /// Converts a guest user to a normal one with passed information in background.
        /// <para>You should get the user object by <see cref="GetCurrentUser"/> because it contains the
        /// information which backtory server needs to distinct the guest user from others.</para>
        /// <para>This is preferable to using <see cref="CompleteRegistration(GuestCompletionParam)"/>,
        /// unless your code is already running from a background thread.</para>
        /// </summary>
        /// <param name="oldPassword">old password user wants to change</param>
        /// <param name="newPassword">newPassword new password user wants to change old one into it.</param>
        /// <exception cref="InvalidOperationException">If you call this on a guest user</exception>
        /// <param name="callback"></param>
        public void ChangePasswordInBackground(string oldPassword, string newPassword, Action<BacktoryResponse<object>> callback)
        {
            if (Guest)
                throw new InvalidOperationException("guest user con not change it's password");
            Backtory.ExecuteRequestAsync(ChangePassRequest(oldPassword, newPassword), callback);
        }

        /// <summary>
        /// Synchronously changes the user password in backtory server and returns its response.
        /// <para>Typically, you should use <see cref="ChangePasswordInBackground(string, string, Action{BacktoryResponse{object}})"/>
        /// instead of this, unless you are managing your own threading.</para>
        /// <para><b>You can't change a guest users's password.</b></para>
        /// </summary>
        /// <param name="oldPassword">old password user wants to change</param>
        /// <param name="newPassword">newPassword new password user wants to change old one into it.</param>
        /// <exception cref="InvalidOperationException">If you call this on a guest user</exception>
        /// <returns>Server response containing no body</returns>
        public BacktoryResponse<object> ChangePassword(string oldPassword, string newPassword)
        {
            if (Guest)
                throw new InvalidOperationException("guest user con not change it's password");
            return Backtory.ExecuteRequest<object>(ChangePassRequest(oldPassword, newPassword));
        }
        #endregion

        #region update user
        private IRestRequest UpdateUserRequest(BacktoryUser toBeUpdateUser)
        {
            var request = Backtory.RestRequest("auth/users/{user_id}", Method.PUT);
            request.AddHeader(Backtory.AuthInstanceIdString, BacktoryConfig.BacktoryAuthInstanceId);
            request.AddHeader(KeyAuthorization, AuthorizationHeader());
            request.AddParameter("user_id", toBeUpdateUser.UserId, ParameterType.UrlSegment);
            request.AddJsonBody(toBeUpdateUser);
            return request;
        }

        /// <summary>
        /// Updates this user changes in backtory server in background.
        /// <para>This is preferable to using <see cref="UpdateUser"/>
        /// unless your code is already running from a background thread.</para>
        /// </summary>
        /// <param name="callback">backtoryCallBack callback notified upon receiving server response or any error in the
        /// process. Server response is updated user wrapped in a <c>BacktoryRespone</c></param>
        public void UpdateUserInBackground(Action<BacktoryResponse<BacktoryUser>> callback)
        {
            Backtory.ExecuteRequestAsync<BacktoryUser>(UpdateUserRequest(this), updateResponse =>
            {
                if (updateResponse.Successful)
                {
                    SaveAsCurrentUserInMemoryAndStorage(updateResponse.Body);
                }
                callback(updateResponse);
            });
        }

        // TODO: check if user been changed in at least on field.
        /// <summary>
        /// Synchronously updates this user changes in backtory server.
        /// <para>Typically, you should use <see cref="UpdateUserInBackground(Action{BacktoryResponse{BacktoryUser}})"/>
        /// instead of this, unless you are managing your own threading.</para>
        /// </summary>
        /// <returns>updated user wrapped in a <c>BacktoryResponse</c></returns>
        public BacktoryResponse<BacktoryUser> UpdateUser()
        {
            var updateResponse = Backtory.ExecuteRequest<BacktoryUser>(UpdateUserRequest(this));
            if (updateResponse.Successful)
                DispatchSaveCurrentUser(updateResponse.Body);
            return updateResponse;
        }
        #endregion

        #region logout
        private static IRestRequest LogoutRequest(string refreshToken)
        {
            var request = Backtory.RestRequest("auth/logout", Method.DELETE);
            request.AddHeader(Backtory.AuthInstanceIdString, BacktoryConfig.BacktoryAuthInstanceId);
            request.AddParameter("refresh-token", refreshToken, ParameterType.QueryString);
            request.OnBeforeDeserialization = response => { response.ContentType = "text/plain"; };
            return request;
        }

        /// <summary>
        /// We must clear everything first, because logout is independent from server and expiration of refresh-token
        /// but if doing that, we can't get refresh token from storage because it's already cleared.
        /// </summary>
        private static string ClearStorageAndReturnRefreshToken()
        {
            var refreshToken = GetRefreshToken();
            ClearBacktoryStoredData();
            return refreshToken;
        }

        /// <summary>
        /// Logouts the current user from backtory server and clears every data related to current
        /// user. After this method <see cref="GetCurrentUser"/> will return <c>null</c>.
        /// <para>This is preferable to using <see cref="Logout"/>, unless your code is already running from a
        /// background thread.</para>
        /// </summary>
        public static void LogoutInBackground()
        {
            Backtory.ExecuteRequestAsync<object>(LogoutRequest(ClearStorageAndReturnRefreshToken()), null);
        }

        /// <summary>
        /// Synchronously logouts the current user from backtory server and clears every data related to
        /// user. After this method <see cref="GetCurrentUser"/> will return <c>null.</c>
        /// <para>Typically, you should use <see cref="LogoutInBackground"/> 
        /// instead of this, unless you are managing your own threading</para>
        /// </summary>
        public static void Logout()
        {
            Backtory.ExecuteRequest<object>(LogoutRequest(ClearStorageAndReturnRefreshToken()));
        }

        internal static void ClearBacktoryStoredData()
        {
            currentUser = null;
            Backtory.Storage.Clear();
        }
        #endregion

        #region properties

        [SerializeAs(Name = "userId")]
        [DeserializeAs(Name = "userId")]
        public string UserId { get; internal set; }

        [SerializeAs(Name = "username")]
        [DeserializeAs(Name = "username")]
        public string Username { get; set; }

        [SerializeAs(Name = "password")]
        [DeserializeAs(Name = "password")]
        public string Password { get; set; }

        [SerializeAs(Name = "firstName")]
        [DeserializeAs(Name = "firstName")]
        public string FirstName { get; set; }

        [SerializeAs(Name = "lastName")]
        [DeserializeAs(Name = "lastName")]
        public string LastName { get; set; }

        [SerializeAs(Name = "email")]
        [DeserializeAs(Name = "email")]
        public string Email { get; set; }

        [SerializeAs(Name = "phoneNumber")]
        [DeserializeAs(Name = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [SerializeAs(Name = "guest")]
        [DeserializeAs(Name = "guest")]
        public bool Guest { get; internal set; }

        [SerializeAs(Name = "active")]
        [DeserializeAs(Name = "active")]
        public bool Active { get; internal set; }

        #endregion

        #region Builder
        public class Builder
        {
            private string firstName;
            private string lastName;
            private string username;
            private string password;
            private string email;
            private string phoneNumber;

            public Builder SetFirstName(string firstName)
            {
                this.firstName = firstName;
                return this;
            }

            public Builder SetLastName(string lastName)
            {
                this.lastName = lastName;
                return this;
            }

            public Builder SetUsername(string username)
            {
                this.username = username;
                return this;
            }

            public Builder SetPassword(string password)
            {
                this.password = password;
                return this;
            }

            public Builder SetEmail(string email)
            {
                this.email = email;
                return this;
            }

            public Builder SetPhoneNumber(string phoneNumber)
            {
                this.phoneNumber = phoneNumber;
                return this;
            }

            public BacktoryUser build()
            {
                return new BacktoryUser(
                    firstName, lastName,
                    Utils.checkNotNull(username, "user name can not be null"),
                    password, email, phoneNumber);
            }
        }
        #endregion

        #region constructors
        public BacktoryUser() { }
        public BacktoryUser(string firstName, string lastName, string username,
                          string password, string email, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
        }
        #endregion

        #region login response POCO
        public class LoginResponse
        {
            [DeserializeAs(Name = "access_token")]
            public string AccessToken { get; internal set; }

            [DeserializeAs(Name = "type_token")]
            public string TypeToken { get; internal set; }

            [DeserializeAs(Name = "refresh_token")]
            public string RefreshToken { get; internal set; }

            [DeserializeAs(Name = "expires_in")]
            public string ExpiresIn { get; internal set; }

            [DeserializeAs(Name = "scope")]
            public string scope { get; internal set; }

            [DeserializeAs(Name = "jti")]
            public string jti { get; internal set; }
        }
        #endregion

        #region guest completion parameters POCO
        public class GuestCompletionParam
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string LastPassword { get; set; }
            public string NewUsername { get; set; }
            public string NewPassword { get; set; }
            public string Email { get; set; }
        }
        #endregion

        #region move to BacktoryAuth class
        internal static string AuthorizationHeader()
        {
            string accessToken = GetAccessToken();
            return accessToken != null ? "Bearer " + accessToken : null;
        }

        internal static string GetAccessToken()
        {
            try
            {
                return GetLoginResponse().AccessToken;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        internal static string GetRefreshToken()
        {
            try
            {
                return GetLoginResponse().RefreshToken;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        internal static LoginResponse GetLoginResponse()
        {
            // TODO: store in ram to prevent every time deserialization
            return GetStoredLoginResponse();
        }

        private static LoginResponse GetStoredLoginResponse()
        {
            string loginResponseString = Backtory.Storage.Get(KeyLoginInfo);
            /*if (loginResponseString == null)
              throw new IllegalStateException("no auth token exists");*/
            return Backtory.FromJson<LoginResponse>(loginResponseString);
        }

        internal static string GetGuestPassword()
        {
            return Backtory.Storage.Get(KeyGuestPassword);
        }
        #endregion
    }
}