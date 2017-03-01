using System;
using Backtory.Core.Public;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.UI;
using Backtory.Core.Internal;
using System.Runtime.InteropServices;
using System.Collections.Generic;
// using UnityEditor;

namespace Assets.BacktorySample
{

	public class Sample : MonoBehaviour
	{
		public Text ResultText;
		private int CoinValue;
		private int TimeValue;
		public Text CoinText;
		public Text TimeText;
		public InputField chosenFilePath;

		void Awake ()
		{

			//GlobalEventListener l = new GlobalEventListener();
			//l.resultText = this.ResultText;
			//BacktoryManager.Instance.GlobalEventListener = l;
//	      Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
//	      Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
//	      Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
//	      Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.Full);
		}

		void OnGUI ()
		{
			// GUILayout.Label("Hello world ");
		}
		// Use this for initialization
		void Start ()
		{
			//Debug.Log ("Alireza: " + Debug.isDebugBuild);
			//Debug.Log ("Alireza: " + BacktoryClient.DebugMode);
			BacktoryClient.DebugMode = true;

			RefreshTimeCoin ();
		}

		// Update is called once per frame
		void Update ()
		{

		}

		#region click listeners

		public void onGuestRegisterClick ()
		{
			BacktoryUser.LoginAsGuestInBackground (response => ResultText.text = response.Successful ? "succeeded" : "failed; " + response.Message);
		}

		public void onRegisterClick ()
		{
			new BacktoryUser.Builder ().SetFirstName ("Alireza").
          		SetLastName ("Farahani").
          		SetUsername (GenerateUsername (true)).
          		SetEmail (GenerateEmail (true)).
          		SetPassword (GeneratePassword (true)).
          		SetPhoneNumber ("09121234567").
          		build ().RegisterInBackground (PrintCallBack<BacktoryUser> ());
		}

		public void onLoginClick ()
		{
			BacktoryUser.LoginInBackground (LastGenUsername, LastGenPassword, response => ResultText.text = response.Successful ? "succeeded" : "failed; " + response.Message);
		}

		public void onCurrentUserClick ()
		{
			ResultText.text = JsonConvert.SerializeObject (BacktoryUser.GetCurrentUser (), Formatting.Indented, JsonnetSetting ());
		}

		public void onCompleteRegistration ()
		{
			LastGenUsername = GenerateUsername (true);
			LastGenPassword = "guest pass";
			BacktoryUser.GetCurrentUser ().CompleteRegistrationInBackgrond (new BacktoryUser.GuestCompletionParam () {
				FirstName = "not guest",
				LastName = "not guest last name",
				Email = GenerateEmail (true),
				NewPassword = LastGenPassword,
				NewUsername = LastGenUsername
			}, PrintCallBack<BacktoryUser> ());
		}

		public void onChangePassword ()
		{
			//BacktoryUser.NewAccessTokenInBackground(PrintCallBack<BacktoryUser.LoginResponse>());
			BacktoryUser.GetCurrentUser ().ChangePasswordInBackground (LastGenPassword, "4321", changePassResponse => {
				ResultText.text = changePassResponse.Successful ? "succeeded" : "failed; " + changePassResponse.Message;
			});
		}

		public void onUpdateUser ()
		{
			var user = BacktoryUser.GetCurrentUser ();
			user.FirstName = "edit";
			user.LastName = "edit manesh";
			user.Username = GenerateUsername (true);
			user.Email = GenerateEmail (true);
			user.PhoneNumber = "22222222";
			user.UpdateUserInBackground (PrintCallBack<BacktoryUser> ());
		}

		public void onLogout ()
		{
			BacktoryUser.LogoutInBackground ();
			ResultText.text = "successfully logged out";
		}

		public void onEchoCloudCode ()
		{
			BacktoryCloudcode.RunInBackground ("echo", "body body body!", PrintCallBack<string> ());
		}

		public void onSearchCloudCode ()
		{
			BacktoryCloudcode.RunInBackground<Person> ("hello", new Info () { id = "453" }, response => {
				if (response.Successful) {
					Person person = response.Body;
					ResultText.text = "search result\nname: " + person.name;
				} else
					ResultText.text = "search failed\n" + response.Code + " " + ((BacktoryHttpStatusCode)response.Code).ToString ();
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

		public void onSendEvent ()
		{
			new GameOverEvent (CoinValue, TimeValue).SendInBackground (response => ResultText.text = response.Successful ? "succeeded" : "failed");
			RefreshTimeCoin ();
		}

		private void RefreshTimeCoin ()
		{
			CoinValue = UnityEngine.Random.Range (0, 100);
			TimeValue = UnityEngine.Random.Range (0, 200);
			CoinText.text = "coin: " + CoinValue.ToString ();
			TimeText.text = "time: " + TimeValue.ToString ();
		}

		public void onGetPlayerRank ()
		{
			new TopPlayersLeaderBoard ().GetPlayerRankInBackground (PrintCallBack<BacktoryLeaderBoard.LeaderBoardRank> ());
		}

		public void onGetTopPlayers ()
		{
			new TopPlayersLeaderBoard ().GetTopPlayersInBackground (2, PrintCallBack<BacktoryLeaderBoard.LeaderBoardResponse> ());
		}

		public void onAroundMePlayers ()
		{
			new TopPlayersLeaderBoard ().GetPlayersAroundMeInBackground (2, PrintCallBack<BacktoryLeaderBoard.LeaderBoardResponse> ());
		}






		/*
		 * Storage
		 */
		public void chooseFile() {
//			string path = EditorUtility.OpenFilePanel ("Choose Your File.", "~/Desktop/4plus_update/assets/", "png");
//			if (path.Length != 0) {
//				chosenFilePath.text = path;
//			}
			chosenFilePath.text = "/Users/mohammad/Desktop/4plus_update/assets/kalkal_icon.png";
		}

		private string filePathAtServer;

		public void uploadFile() {
			var bf = new BacktoryFile ();
			bf.UploadInBackground (chosenFilePath.text, "tmp", true, (response) => {
				ResultText.text = response.Successful ? "upload file succeeded.\n" + 
					JsonConvert.SerializeObject (response.Body, Formatting.Indented, JsonnetSetting ()) : "failed; " + response.Message;
				if (response.Successful)
					filePathAtServer = response.Body;
			});
		}

		public void renameFile() {
//			if (filePathAtServer == null) {
//				ResultText.text = "No file path at server available!";
//				return;
//			}
			var bf = new BacktoryFile (filePathAtServer);
			bf.RenameInBackground ("another_name.png", (response) => {
				ResultText.text = response.Successful ? "rename file succeeded.\n" + 
					JsonConvert.SerializeObject (response.Body, Formatting.Indented, JsonnetSetting ()) : "failed; " + response.Message;
//				if (response.Successful)
//					filePathAtServer = response.Body;
			});
		}

		public void deleteFile() {
//			if (filePathAtServer == null) {
//				ResultText.text = "No file path at server available!";
//				return;
//			}
			var bf = new BacktoryFile (filePathAtServer);
			bf.DeleteInBackground (false, (response) => {
				ResultText.text = response.Successful ? "delete file succeeded." : "failed; " + response.Message;
				if (response.Successful)
					filePathAtServer = null;
			});
		}

		public void uploadMultipleFile() {
			var bf = new BacktoryFile.Bulk ();
		}

		/* 
		 * Matchmaking
		 */
		BacktoryMatchMaking mm;

		public void loginMMUser1() {
			BacktoryUser.LoginInBackground("testUser", "12341234", (IBacktoryResponse response) => {
				ResultText.text = response.Successful ? "matchmaking user 1 login succeeded" : "failed; " + response.Message;
			});		
		}

		public void loginMMUser2() {
			BacktoryUser.LoginInBackground("testUser2", "12341234", (IBacktoryResponse response) => {
				ResultText.text = response.Successful ? "matchmaking user 2 login succeeded" : "failed; " + response.Message;
			});		
		}

		public void realtimeConnect() {
			BacktoryClient.StartRealtimeService (new BacktoryConnectionStatusListener() {
				OnOpen = () => {
					ResultText.text = "Realtime service started successfully!";
				},
				OnClose = () => {
					ResultText.text = "Realtime service stopped successfully!";
				},
				OnError = (message) => {
					ResultText.text = "start realtime error!" + message;
				}
			});
			BacktoryClient.SetOnErrorListener ((errorMsg) => {
				ResultText.text = JsonConvert.SerializeObject (errorMsg, Formatting.Indented, JsonnetSetting ());
			});
		}

		public void realtimeDisconnect() {
			BacktoryClient.StopRealtimeService ();
		}

		public void requestMatch() {
			mm = new BacktoryMatchMaking ("matchmaking1", 100);
			mm.OnMatchFound = (match) => {
				ResultText.text = "Match found!\n" + JsonConvert.SerializeObject (match, Formatting.Indented, JsonnetSetting ());
				setupRealtimeGame(match);
			};
			mm.OnPlayerAddedToMatchMaking = (participantList) => {
				ResultText.text = "Player added to matchmaking!\n" + JsonConvert.SerializeObject (participantList, Formatting.Indented, JsonnetSetting ());
			};
			mm.OnMatchNotFound = (message) => {
				ResultText.text = "Match not found!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
			};

			mm.Request ("Guess who is back.", (response) => {
				ResultText.text = response.Successful ? "Request match succeeded." : "failed; " + response.Message;
			});
		}

		public void cancelMatchRequest() {
			if (mm != null) {
				mm.Cancel ((response) => {
					ResultText.text = response.Successful ? "Canceling request match succeeded." :
						"failed; " + response.Message;
				});
			} else {
				ResultText.text = "No matchmaking available!";
			}
		}




		/* 
		 * Challenge
		 */
		public void loginChallengeUser1() {
			BacktoryUser.LoginInBackground("testUser", "12341234", (IBacktoryResponse response) => {
				ResultText.text = response.Successful ? "challenge user 1 login succeeded" : "failed; " + response.Message;
				setChallengeListeners();
			});
		}

		public void loginChallengeUser2() {
			BacktoryUser.LoginInBackground("testUser2", "12341234", (IBacktoryResponse response) => {
				ResultText.text = response.Successful ? "challenge user 2 login succeeded" : "failed; " + response.Message;
				setChallengeListeners();
			});		
		}

		BacktoryChallenge invitedChallenge;

		private void setChallengeListeners() {
			BacktoryChallenge.SetOnInvitingToChallengeListener((message) => {
				ResultText.text = "Invited to a challenge!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				invitedChallenge = message;
			});
			BacktoryChallenge.SetOnChallengeFailedListener((message) => {
				ResultText.text = "Challenge failed!\n" + JsonConvert.SerializeObject (message.Cause, Formatting.Indented, JsonnetSetting ());
			});
			BacktoryChallenge.SetOnChallengeAcceptedListener((message) => {
				ResultText.text = "Challenge accepted!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
			});
			BacktoryChallenge.SetOnChallengeRejectedListener((message) => {
				ResultText.text = "Challenge rejected!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
			});
			BacktoryChallenge.SetOnChallengeReadyListener((match) => {
				ResultText.text = "Challenge is ready!\n" + JsonConvert.SerializeObject (match, Formatting.Indented, JsonnetSetting ());
				setupRealtimeGame(match);
			});		
		}

		BacktoryChallenge requestedChallenge;

		public void requestChallenge() {
			IList<string> challengedUsers = new List<string>{"58a1d403e4b0d88fcd9f469f"};	// TestUser1
			BacktoryChallenge.CreateNew(challengedUsers, 2, 25, (response) => {
				if (response.Successful) {
					ResultText.text = "New challenge requested!\n" + JsonConvert.SerializeObject (response.Body, Formatting.Indented, JsonnetSetting ());
					requestedChallenge = response.Body;
				} else {
					ResultText.text = response.Message;
				}
			});
			// TODO tell alireza about parameters order in Android and Unity!
		}

		public void cancelChallenge() {
			if (requestedChallenge == null) {
				ResultText.text = "No requested challenge available!";
				return;
			}
			requestedChallenge.Cancel ((response) => {
				ResultText.text = response.Successful ? "cancel challenge request succeeded." : "failed; " + response.Message;
			});
		}

		public void acceptChallenge() {
			if (invitedChallenge == null) {
				ResultText.text = "No invited challenge available!";
				return;
			}
			invitedChallenge.Accept ("myMetaData", (response) => {
				ResultText.text = response.Successful ? "accept challenge invitation succeeded." : "failed; " + response.Message;
			});
		}

		public void rejectChallenge() {
			if (invitedChallenge == null) {
				ResultText.text = "No invited challenge available!";
				return;
			}
			invitedChallenge.Reject ((response) => {
				ResultText.text = response.Successful ? "reject challenge invitation succeeded." : "failed; " + response.Message;
			});
		}

		public void requestActiveChallenges() {
			BacktoryChallenge.ActiveChallengs (PrintCallBack<IList<BacktoryChallenge>>());
		}




		/* 
		 * Realtime 
		 */
		BacktoryRealtimeGame realtimeGame;

		private void setupRealtimeGame(BacktoryMatch match) {
			realtimeGame = new BacktoryRealtimeGame (match);
			realtimeGame.OnAPlayerMove = (message) => {
				ResultText.text = JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
			};
			realtimeGame.OnDirectMessage = (message) => {
				ResultText.text = "Direct chat received.\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
			};
			realtimeGame.OnError = (message) => {
				ResultText.text = "Error occurred!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
			};
			realtimeGame.OnGameEnded = (message) => {
				ResultText.text = "Game ended!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
			};
			realtimeGame.OnGameStarted = () => {
				ResultText.text = "Game started !!!!!!!!!!";
			};
			realtimeGame.OnPlayerJoined = (message) => {
				ResultText.text = "Player joined the game!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
			};
			realtimeGame.OnPlayerLeft = (message) => {
				ResultText.text = "Player left the game!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
			};
			realtimeGame.OnPublicMessage = (message) => {
				ResultText.text = "Public message received!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
			};
			realtimeGame.OnServerMessage = (message) => {
				ResultText.text = "Server message received!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
			};
		}

		public void connectToMatch() {
			if (realtimeGame == null) {
				ResultText.text = "No match is available.";
				return;
			}
			realtimeGame.Join ();
		}

		public void sendEvent() {
			if (realtimeGame == null) {
				ResultText.text = "No match is available.";
				return;
			}
			Dictionary<string, string> data = new Dictionary<string, string>();
			data.Add ("myKey", "myValue");
			realtimeGame.SendPlayerMoves("myMessage", data);
		}

		public void directMessage() {
			if (realtimeGame == null) {
				ResultText.text = "No match is available.";
				return;
			}
			realtimeGame.SendDirectMessage("58a1d423e4b09c2c6a51de27",	// TestUser2
				"Hello, what's up?!", (response) => {
				ResultText.text = response.Successful ? "send direct chat in match succeeded." : "failed; " + response.Message;
			});
		}

		public void sendChatToMatch() {
			if (realtimeGame == null) {
				ResultText.text = "No match is available.";
				return;
			}
			realtimeGame.SendPublicMessage ("Please pause. :)", (response) => {
				ResultText.text = response.Successful ? "send public message in match succeeded." : "failed; " + response.Message;
			});
		}

		public void sendMatchResult() {
			if (realtimeGame == null) {
				ResultText.text = "No match is available.";
				return;
			}
			IList<string> winners = new List<string>{"58a1d403e4b0d88fcd9f469f"};	// TestUser1
			realtimeGame.SendWinners (winners, "myExtraData", (response) => {
				ResultText.text = response.Successful ? "send winners in match succeeded." : "failed; " + response.Message;
			});
		}




		/* 
		 * Person-to-Person Chat 
		 */
		public void loginChatUser() {
			BacktoryUser.LoginInBackground("testUser", "12341234", (IBacktoryResponse response) => {
				ResultText.text = response.Successful ? "chat user login succeeded" : "failed; " + response.Message;
				BacktoryChat.Direct.SetOnReceivingMessageListener ((BacktoryDirectChatMessage message) => {
					ResultText.text = JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				});
				BacktoryChat.Group.SetOnInvitingToJoinListener((message) => {
					ResultText.text = "Invited to join!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
					invitedGroupId = message.GroupId;
				});
				BacktoryChat.Group.SetOnMemberAddedListener((message) => {
					ResultText.text = "Member added to group!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				});
				BacktoryChat.Group.SetOnMemberJoinedListener((message) => {
					ResultText.text = "Member joined the group!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				});
				BacktoryChat.Group.SetOnMemberLeftListener((message) => {
					ResultText.text = "Member left the group!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				});
				BacktoryChat.Group.SetOnMemberRemovedListener((message) => {
					ResultText.text = "Member is removed from group!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				});
				BacktoryChat.Group.SetOnReceivingMessageListener((message) => {
					ResultText.text = "Message received from group chat!\n" + JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				});
			});
		}
			
		public void sendChatMessage() {
			int id = UnityEngine.Random.Range (0, 10);
			BacktoryChat.Direct dc = new BacktoryChat.Direct ("58a1d403e4b0d88fcd9f469f");	// TestUser1
			dc.SendMessage ("Working! " + id, (response) => {
				ResultText.text = response.Successful ? "sending chat succeeded" : "failed; " + response.Message;
			});
		}

		public void requestOfflineChats() {
			BacktoryChat.OfflineMessages (PrintCallBack<IList<AbsChatMessage>>());
		}

		public static long CurrentTimeMillis()
		{
			DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (long) (DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
		}

		public void requestChatHistory () {
			BacktoryChat.Direct dc = new BacktoryChat.Direct ("58a1d423e4b09c2c6a51de27");	// TestUser2
			dc.History (CurrentTimeMillis(), PrintCallBack<IList<BacktoryDirectChatMessage>>());
		}


		/* 
		 * Group Chat 
		 */
		public void createChatGroup() {
			int id = UnityEngine.Random.Range (0, 10);
			BacktoryChat.Group.CreateNewGroup ("MyGroup" + id, BacktoryChat.Group.Mode.Private,
				PrintCallBack<BacktoryChat.Group>());
		}

		private IList<BacktoryChat.Group> chatGroupList;

		public void requestGroupsList() {
			BacktoryChat.Group.MyGroups ((response) => {
				if (response.Successful)	
					ResultText.text = JsonConvert.SerializeObject (response.Body, Formatting.Indented, JsonnetSetting ());
				else
					ResultText.text = response.Message;
				chatGroupList = response.Body;
			});
		}

		public void requestMembersList() {
			if (chatGroupList == null || chatGroupList.Count == 0) {
				ResultText.text = "List of chat groups is null or empty.";
				return;
			}
			int id = UnityEngine.Random.Range (0, chatGroupList.Count);
			var bgc = new BacktoryChat.Group (chatGroupList[id].GroupId);
			bgc.MembersInfo (PrintCallBack<BacktoryGroupMembersInfoResponse>());

		}

		public void addGroupMember() {
			if (chatGroupList == null || chatGroupList.Count == 0) {
				ResultText.text = "List of chat groups is null or empty.";
				return;
			}
			int id = UnityEngine.Random.Range (0, chatGroupList.Count);
			var bgc = new BacktoryChat.Group (chatGroupList[id].GroupId);
			bgc.AddMember ("58a1d403e4b0d88fcd9f469f", (response) => { // TestUser1
				ResultText.text = response.Successful ? "Add group member succeeded." : "failed; " + response.Message;
			});	
		}

		public void removeGroupMember() {
			if (chatGroupList == null || chatGroupList.Count == 0) {
				ResultText.text = "List of chat groups is null or empty.";
				return;
			}
			int id = UnityEngine.Random.Range (0, chatGroupList.Count);
			var bgc = new BacktoryChat.Group (chatGroupList[id].GroupId);
			bgc.RemoveMember ("58a1d403e4b0d88fcd9f469f", (response) => {	// TestUser1
				ResultText.text = response.Successful ? "Remove group member succeeded." : "failed; " + response.Message;
			});
		}

		public void sendChatToGroup() {
			if (chatGroupList == null || chatGroupList.Count == 0) {
				ResultText.text = "List of chat groups is null or empty.";
				return;
			}
			int id = UnityEngine.Random.Range (0, chatGroupList.Count);
			var bgc = new BacktoryChat.Group (chatGroupList[id].GroupId);
			bgc.SendMessage ("Hello Everybodyyyyy!", (response) => {
				ResultText.text = response.Successful ? "Send chat to group succeeded." : "failed; " + response.Message;
			});
		}

		public void requestGroupChatHistory() {
			if (chatGroupList == null || chatGroupList.Count == 0) {
				ResultText.text = "List of chat groups is null or empty.";
				return;
			}
			int id = UnityEngine.Random.Range (0, chatGroupList.Count);
			var bgc = new BacktoryChat.Group (chatGroupList[id].GroupId);
			bgc.History(CurrentTimeMillis(), PrintCallBack<IList<AbsGroupChatMessage>>());
		}

		string invitedGroupId;

		public void inviteToGroup() {
			if (chatGroupList == null || chatGroupList.Count == 0) {
				ResultText.text = "List of chat groups is null or empty.";
				return;
			}
			int id = UnityEngine.Random.Range (0, chatGroupList.Count);
			var bgc = new BacktoryChat.Group (chatGroupList[id].GroupId);
			bgc.InviteToGroup ("58a1d403e4b0d88fcd9f469f",  (response) => {		//TestUser1
				ResultText.text = response.Successful ? "Invitation to group chat succeeded." : "failed; " + response.Message;
			});
		}

		public void joinGroup() {
			if (invitedGroupId == null) {
				ResultText.text = "Invited group id is null.";
				return;
			}
			var bgc = new BacktoryChat.Group (invitedGroupId);
			bgc.JoinGroup ((response) => {
				ResultText.text = response.Successful ? "Joined the group chat successfully!" : "failed; " + response.Message;
				invitedGroupId = null;
			});
		}

		public void leaveGroup() {
			if (chatGroupList == null || chatGroupList.Count == 0) {
				ResultText.text = "List of chat groups is null or empty.";
				return;
			}
			int id = UnityEngine.Random.Range (0, chatGroupList.Count);
			var bgc = new BacktoryChat.Group (chatGroupList[id].GroupId);
			bgc.LeaveGroup ((response) => {
				ResultText.text = response.Successful ? "Left the group chat successfully!" : "failed; " + response.Message;
			});
		}

		public void makeMemberOwner() {
			if (chatGroupList == null || chatGroupList.Count == 0) {
				ResultText.text = "List of chat groups is null or empty.";
				return;
			}
			int id = UnityEngine.Random.Range (0, chatGroupList.Count);
			var bgc = new BacktoryChat.Group (chatGroupList[id].GroupId);
			bgc.MakeMemberOwner ("58a1d403e4b0d88fcd9f469f",(response) => {			// TestUser1
				ResultText.text = response.Successful ? "Member is owner now!" : "failed; " + response.Message;
			});
		}

		#endregion

		#region sample stuff

		internal const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		private static string LastGenEmail;

		private static string LastGenUsername {
			get {
				return PlayerPrefs.GetString ("last username");
			}
			set {
				PlayerPrefs.SetString ("last username", value);
			}
		}

		private static string LastGenPassword {
			get {
				return PlayerPrefs.GetString ("last password");
			}
			set {
				PlayerPrefs.SetString ("last password", value);
			}
		}

		private static string RandomAlphabetic (int length)
		{
			var charArr = new char[length];
			//var random = new System.Random(Environment.TickCount);
			for (int i = 0; i < charArr.Length; i++) {
				//charArr[i] = chars[random.Next()];
				charArr [i] = chars [UnityEngine.Random.Range (0, chars.Length)];
			}
			return new string (charArr);
		}

		internal static string GenerateEmail (bool random)
		{
			string s = random ? RandomAlphabetic (3) + "@" + RandomAlphabetic (3) + ".com" : "ar.d.farahani@gmail.com";
			LastGenEmail = s;
			return s;
		}

		internal static string GenerateUsername (bool random)
		{
			string s = random ? RandomAlphabetic (6) : "hamze";
			LastGenUsername = s;
			return s;
		}

		internal static string GeneratePassword (bool random)
		{
			string s = random ? RandomAlphabetic (6) : "1234";
			LastGenPassword = s;
			return s;
		}

		internal Action<IBacktoryResponse<T>> PrintCallBack<T> ()
		{
			return (backtoryResponse) => {
				if (backtoryResponse.Successful)
					ResultText.text = JsonConvert.SerializeObject (backtoryResponse.Body, Formatting.Indented, JsonnetSetting ());
				else
					ResultText.text = backtoryResponse.Message;
			};
		}

		private JsonSerializerSettings JsonnetSetting ()
		{
			return new JsonSerializerSettings () {
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore,
				DefaultValueHandling = DefaultValueHandling.Include,
				ContractResolver = new CamelCasePropertyNamesContractResolver (),
			};
		}
		//public class GlobalEventListener : IGlobalEventListener
		//{
		//  public Text resultText { set; get; }
		//  public void OnEvent(BacktorySDKEvent logoutEvent)
		//  {
		//    if (logoutEvent is LogoutEvent)
		//      resultText.text = "you must login again!";
		//  }
		//}
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
