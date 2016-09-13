using Assets.BacktorySDK;
using Assets.BacktorySDK.auth;
using Assets.BacktorySDK.cloudcode;
using Assets.BacktorySDK.core;
using Assets.BacktorySDK.game;
using RestSharp.Serializers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.BacktorySample
{
    
    public class Sample : MonoBehaviour
    {
        public Text ResultText;
        private int CoinValue;
        private int TimeValue;
        public Text CoinText;
        public Text TimeText;

        void Awake()
        {
            GlobalEventListener l = new GlobalEventListener();
            l.resultText = this.ResultText;
            BacktoryManager.Instance.GlobalEventListener = l;
        }

        void OnGUI()
        {
            // GUILayout.Label("Hello world ");
        }
        // Use this for initialization
        void Start()
        {
            RefreshTimeCoin();
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region click listeners
        public void onGuestRegisterClick()
        {
            //BacktoryUser.LoginAsGuestInBackground(response =>
            //{
            //    ResultText.text = response.Body.AccessToken;
            //    Debug.Log("result set");
            //});
            BacktoryUser.LoginAsGuestInBackground(PrintCallBack<BacktoryUser.LoginResponse>());
        }

        public void onRegisterClick()
        {
            new BacktoryUser.Builder().SetFirstName("Alireza").
                SetLastName("Farahani").
                SetUsername(GenerateUsername(true)).
                SetEmail(GenerateEmail(true)).
                SetPassword(GeneratePassword(true)).
                SetPhoneNumber("09121234567").
                build().RegisterInBackground(PrintCallBack<BacktoryUser>());
        }

        public void onLoginClick()
        {
            BacktoryUser.LoginInBackground(LastGenUsername, LastGenPassword, PrintCallBack<BacktoryUser.LoginResponse>());
        }

        public void onCurrentUserClick()
        {
            ResultText.text = Backtory.ToJson(BacktoryUser.GetCurrentUser(), true);
        }

        public void onCompleteRegistration()
        {
            LastGenUsername = GenerateUsername(true);
            LastGenPassword = "guest pass";
            BacktoryUser.GetCurrentUser().CompleteRegistrationInBackgrond(new BacktoryUser.GuestCompletionParam()
            {
                FirstName = "not guest",
                LastName = "not guest last name",
                Email = GenerateEmail(true),
                NewPassword = LastGenPassword,
                NewUsername = LastGenUsername
            }, PrintCallBack<BacktoryUser>());
        }

        public void onChangePassword()
        {
            //BacktoryUser.NewAccessTokenInBackground(PrintCallBack<BacktoryUser.LoginResponse>());
            BacktoryUser.GetCurrentUser().ChangePasswordInBackground(LastGenPassword, "4321", changePassResponse =>
            {
                ResultText.text = changePassResponse.Successful ? "succeeded" : "failed; " + changePassResponse.Message;
            });
        }
        public void onUpdateUser()
        {
            var user = BacktoryUser.GetCurrentUser();
            user.FirstName = "edit";
            user.LastName = "edit manesh";
            user.Username = GenerateUsername(true);
            user.Email = GenerateEmail(true);
            user.PhoneNumber = "22222222";
            user.UpdateUserInBackground(PrintCallBack<BacktoryUser>());
        }

        public void onLogout()
        {
            BacktoryUser.LogoutInBackground();
            ResultText.text = "successfully logged out";
        }

        public void onEchoCloudCode()
        {
            BacktoryCloudcode.RunInBackground("echo", "body body body!", PrintCallBack<string>());
        }

        public void onSearchCloudCode()
        {
            BacktoryCloudcode.RunInBackground<Person>("hello", new Info() { id = "453" }, response =>
            {
                if (response.Successful)
                {
                    Person person = response.Body;
                    ResultText.text = "search result\nname: " + person.name;
                }
                else
                    ResultText.text = "search failed\n" + response.Code + " " + ((BacktoryHttpStatusCode)response.Code).ToString();
            });
        }

        public class Info
        {
            public string id { get; set; }
        }

        public class Person
        {
            public string name { get; set; }
            public int age { get; set; }
        }

        public void onSendEvent()
        {
            new GameOverEvent(CoinValue, TimeValue).sendAsync(response => ResultText.text = response.Successful ? "succeeded" : "failed");
            RefreshTimeCoin();
        }

        private void RefreshTimeCoin()
        {
            CoinValue = UnityEngine.Random.Range(0, 100);
            TimeValue = UnityEngine.Random.Range(0, 200);
            CoinText.text = "coin: " + CoinValue.ToString();
            TimeText.text = "time: " + TimeValue.ToString();
        }

        public void onGetPlayerRank()
        {
            new TopPlayersLeaderBoard().GetPlayerRankInBackground(PrintCallBack<BacktoryLeaderBoard.LeaderBoardRank>());
        }

        public void onGetTopPlayers()
        {
            new TopPlayersLeaderBoard().GetTopPlayersInBackground(2, PrintCallBack<BacktoryLeaderBoard.LeaderBoardResponse>());
        }

        public void onAroundMePlayers()
        {
            new TopPlayersLeaderBoard().GetPlayersAroundMeInBackground(2, PrintCallBack<BacktoryLeaderBoard.LeaderBoardResponse>());
        }

        #endregion

        #region sample stuff
        internal const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static string LastGenEmail;

        private static string LastGenUsername
        {
            get
            {
                return PlayerPrefs.GetString("last username");
            }
            set
            {
                PlayerPrefs.SetString("last username", value);
            }
        }
        private static string LastGenPassword
        {
            get
            {
                return PlayerPrefs.GetString("last password");
            }
            set
            {
                PlayerPrefs.SetString("last password", value);
            }
        }

        private static string RandomAlphabetic(int length)
        {
            var charArr = new char[length];
            //var random = new System.Random(Environment.TickCount);
            for (int i = 0; i < charArr.Length; i++)
            {
                //charArr[i] = chars[random.Next()];
                charArr[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
            }
            return new string(charArr);
        }

        internal static string GenerateEmail(bool random)
        {
            string s = random ? RandomAlphabetic(3) + "@" + RandomAlphabetic(3) + ".com" : "ar.d.farahani@gmail.com";
            LastGenEmail = s;
            return s;
        }

        internal static string GenerateUsername(bool random)
        {
            string s = random ? RandomAlphabetic(6) : "hamze";
            LastGenUsername = s;
            return s;
        }

        internal static string GeneratePassword(bool random)
        {
            string s = random ? RandomAlphabetic(6) : "1234";
            LastGenPassword = s;
            return s;
        }

        internal static readonly ISerializer jsonSerializer = new NewtonsoftJsonSerializer();/*new MyJsonSerializer();*/
        internal Action<BacktoryResponse<T>> PrintCallBack<T>() where T : class
        {
            return (backtoryResponse) =>
            {
                if (backtoryResponse.Successful)
                    ResultText.text = Backtory.ToJson(backtoryResponse.Body, true); /*JsonHelper.FormatJson(jsonSerializer.Serialize(backtoryResponse.Body));*/
                else
                    ResultText.text = backtoryResponse.Message;
            };
        }

        public class GlobalEventListener : IGlobalEventListener
        {
            public Text resultText { set; get; }
            public void OnEvent(BacktorySDKEvent logoutEvent)
            {
                if (logoutEvent is LogoutEvent)
                    resultText.text = "you must login again!";
            }
        }
        #endregion


        //public void onGuestRegisterClick()
        //{
        //    UnityWebRequest.Get("").Send();
        //    StartCoroutine(GuestRegister());

        //}

        //IEnumerator GuestRegister()
        //{
        //    UnityWebRequest guestLoginRequest = new BacktoryUser().LoginAsGuest();
        //    guestLoginRequest.SetRequestHeader(Backtory.ContentTypestring, Backtory.ApplicationJson);
        //    guestLoginRequest.SetRequestHeader(Backtory.AuthIdstring, BacktoryConfig.BacktoryAuthInstanceId);
        //    yield return guestLoginRequest.Send();

        //    if (guestLoginRequest.isError)
        //    {
        //        switch (guestLoginRequest.responseCode)
        //        {
        //            case (int)HttpStatusCode.NotFound:
        //                //TODO: update result textview
        //                Debug.Log(guestLoginRequest.downloadHandler.text);
        //                break;
        //        }
        //    } else
        //    {
        //        Debug.Log(guestLoginRequest.downloadHandler.text);
        //    }
        //}
    }
}
