using System;
using Backtory.Core.Public;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Backtory.InAppPurchase.Public;

namespace Assets.BacktorySample
{
	public class Sample : MonoBehaviour
	{
		// Log Panel
		public Text MessageText;
		public Text ResultText;

		// Auth Module
		public InputField emailInput;
		public InputField usernameInput;
		public InputField passwordInput;
		public InputField newPasswordInput;

		// Game Module
		public InputField CoinInput;
		public InputField TimeInput;

		// File Storage Module
		public InputField fileContentInput;
		public InputField serverFilePathInput;
		public InputField newFileNameInput;

		// Database Module
		public InputField noteTitleInput;
		public InputField notePriorityInput;
		public Toggle notePinnedToggle;

		// Matchmaking Module
		public InputField categoryInput;
		public InputField skillInput;

		// Realtime Module
		public InputField messageInput;

		// CafeBazaar IAP Module
		public Dropdown ItemTypesDropdown;
		public Dropdown SecurityTypesDropdown;
		
		
		
		void Awake ()
		{
		}

		void OnGUI ()
		{
		}

		// Use this for initialization
		void Start ()
		{
		}

		// Update is called once per frame
		void Update ()
		{
		}

		#region click listeners

		/*
		 * Auth
		 */
		public void onGuestLoginClick ()
		{
			BacktoryUser.LoginAsGuestInBackground (response =>
				ResultText.text = response.Successful ? "Login as guest succeeded." : "failed; " + response.Message
			);
		}

		public void onRegisterClick ()
		{
			new BacktoryUser {
				FirstName = "FirstName",
				LastName = "LastName",
				Username = usernameInput.text,
				Email = emailInput.text,
				Password = passwordInput.text,
				PhoneNumber = "09121234567"
			}.RegisterInBackground (PrintCallBack<BacktoryUser> ());
		}

		public void onLoginClick ()
		{
			BacktoryUser.LoginInBackground (usernameInput.text, passwordInput.text, response =>
				ResultText.text = response.Successful ? "Login succeeded." : "failed; " + response.Message
			);
		}

		public void onCurrentUserClick ()
		{
			ResultText.text = JsonConvert.SerializeObject (BacktoryUser.CurrentUser, Formatting.Indented, JsonnetSetting ());
		}

		public void onCompleteRegistration ()
		{
			BacktoryUser.CurrentUser.CompleteRegistrationInBackgrond (new BacktoryUser.GuestCompletionParam () {
				FirstName = "GuestFirstName",
				LastName = "GuestLastName",
				Email = emailInput.text,
				NewPassword = passwordInput.text,
				NewUsername = usernameInput.text
			}, PrintCallBack<BacktoryUser> ());
		}

		public void onChangePassword ()
		{
			if (newPasswordInput.text == "") {
				ResultText.text = "Enter the new password!";
				return;
			}
			BacktoryUser.CurrentUser.ChangePasswordInBackground (passwordInput.text, newPasswordInput.text, changePassResponse => {
				ResultText.text = changePassResponse.Successful ? "Change password succeeded." : "failed; " + changePassResponse.Message;
			});
		}

		public void onForgetPassword ()
		{
			if (usernameInput.text == "") {
				ResultText.text = "Enter username!";
				return;
			}
			BacktoryUser.ForgotPasswordInBackground (usernameInput.text, response => {
				ResultText.text = response.Successful ? "Forgot password succeeded." : "failed; " + response.Message;
			});
		}

		public void onUpdateUser ()
		{
			BacktoryUser user = BacktoryUser.CurrentUser;
			user.FirstName = "UpdatedFirstName";
			user.LastName = "UpdatedLastName";
			user.Username = usernameInput.text;
			user.Email = emailInput.text;
			user.PhoneNumber = "22222222";
			user.UpdateUserInBackground (PrintCallBack<BacktoryUser> ());
		}

		public void onLogout ()
		{
			BacktoryUser.LogoutInBackground ();
			ResultText.text = "Successfully logged out.";
		}



		/*
		 * CloudCode
		 */
		public void onEchoCloudCode ()
		{
			BacktoryCloudcode.RunInBackground ("echo", "Hello World!", PrintCallBack<string> ());
		}

		public void onSearchCloudCode ()
		{
			BacktoryCloudcode.RunInBackground<Person> ("hello", new Info () { Id = "453" }, response => {
				if (response.Successful) {
					Person person = response.Body;
					ResultText.text = "Search result.\nname: " + person.Name;
				} else
					ResultText.text = "Search failed;\n" + response.Code + " " + ((BacktoryHttpStatusCode)response.Code).ToString ();
			});
		}

		public class Info
		{
			public string Id { get; set; }
		}

		public class Person
		{
			public string Name { get; set; }
			public int Age { get; set; }
		}



		/*
		 * Game
		 */
		public void onSendEvent ()
		{
			new GameOverEvent (int.Parse(CoinInput.text), int.Parse(TimeInput.text)).SendInBackground (response => {
				ResultText.text = response.Successful ? "Sending event succeeded." : "failed; " + response.Message;
			});
		}
			
		public void onGetPlayerRank ()
		{
			new TopPlayersLeaderBoard ().GetPlayerRankInBackground (
				PrintCallBack<BacktoryLeaderBoard.LeaderBoardRank> ());
		}

		public void onGetTopPlayers ()
		{
			new TopPlayersLeaderBoard ().GetTopPlayersInBackground (2, 
				PrintCallBack<BacktoryLeaderBoard.LeaderBoardResponse> ());
		}

		public void onAroundMePlayers ()
		{
			new TopPlayersLeaderBoard ().GetPlayersAroundMeInBackground (2,
				PrintCallBack<BacktoryLeaderBoard.LeaderBoardResponse> ());
		}



		/*
		 * File Storage
		 */
		private string filePathOnDevice;

		public void uploadFile() {
			filePathOnDevice = Path.Combine(Application.persistentDataPath, "tmp.txt");
			File.WriteAllText (filePathOnDevice, fileContentInput.text);
			var bf = new BacktoryFile (filePathOnDevice);
			bf.UploadInBackground (serverFilePathInput.text, true, (response) => {
				ResultText.text = response.Successful ? "Upload file succeeded.\n" + 
					JsonConvert.SerializeObject (response.Body, Formatting.Indented, JsonnetSetting ()) : "failed; " + response.Message;
			});
		}

		public void renameFile() {
			BacktoryFile.RenameInBackground (serverFilePathInput.text, newFileNameInput.text, (response) => {
				ResultText.text = response.Successful ? "Rename file succeeded.\n" + 
					JsonConvert.SerializeObject (response.Body, Formatting.Indented, JsonnetSetting ()) : "failed; " + response.Message;
			});
		}

		public void deleteFile() {
			BacktoryFile.DeleteInBackground (serverFilePathInput.text, false, (response) => {
				ResultText.text = response.Successful ? "Delete file succeeded." : "failed; " + response.Message;
			});
		}




		/*
		 * Database (Object Storage)
		 */
		BacktoryObject currentNote;

		public void saveNote() {
			if (currentNote == null) {

				currentNote = new BacktoryObject ("Note");
				currentNote["title"] = noteTitleInput.text;
				currentNote["priority"] = int.Parse (notePriorityInput.text);
				currentNote["pinned"] = notePinnedToggle.isOn;

				currentNote.SaveInBackground (response => {
					if (response.Successful) {
						ResultText.text = "New note created successfully!\n" + currentNote.ToString();
					} else {
						ResultText.text = "failed; " + response.Message;
					}
				});
			} else {
				
				if (noteTitleInput.text != "")
					currentNote["title"] = noteTitleInput.text;
				if (notePriorityInput.text != "")
					currentNote["priority"] = int.Parse(notePriorityInput.text);
				currentNote["pinned"] = notePinnedToggle.isOn;

				currentNote.SaveInBackground (response => {
					if (response.Successful) {
						ResultText.text = "The note updated successfully!\n" + currentNote.ToString();
					} else {
						ResultText.text = "failed; " + response.Message;
					}
				});
			}
		}

		public void deleteNote() {
			if (currentNote == null) {
				ResultText.text = "No note (BacktoryObject) is available!";
				return;
			}
			currentNote.DeleteInBackground (response => {
				if (response.Successful) {
					ResultText.text = "The note deleted successfully.";
					currentNote = null;
				} else {
					ResultText.text = "failed; " + response.Message;
				}
			});
		}

		public void findAllPinnedNotes() {
			BacktoryQuery pinnedQuery = new BacktoryQuery ("Note")
											.WhereEqualTo ("pinned", true);
			pinnedQuery.FindInBackground (response => {
				if (response.Successful) {
					ResultText.text = "Found all pinned notes successfully!\n" + 
								JsonConvert.SerializeObject (response.Body, Formatting.Indented, JsonnetSetting ());
				} else {
					ResultText.text = "failed; " + response.Message;
				}
			});
		}

		public void findTodoTitleNotes() {
			BacktoryQuery todoQuery = new BacktoryQuery ("Note")
								.WhereContains ("title", "todo");
			todoQuery.FindInBackground (response => {
				if (response.Successful) {
					ResultText.text = "Found all notes with title containing 'todo' successfully!\n" + 
								JsonConvert.SerializeObject (response.Body, Formatting.Indented, JsonnetSetting ());
				} else {
					ResultText.text = "failed; " + response.Message;
				}
			});
		}




		/* 
		 * Matchmaking
		 */
		BacktoryMatchMaking mm;

		public void loginMMUser1() {
			BacktoryUser.LoginInBackground(testUser1.username, testUser1.password, (IBacktoryResponse response) => {
				ResultText.text = response.Successful ? "Matchmaking user 1 login succeeded." : "failed; " + response.Message;
			});		
		}

		public void loginMMUser2() {
			BacktoryUser.LoginInBackground(testUser2.username, testUser2.password, (IBacktoryResponse response) => {
				ResultText.text = response.Successful ? "Matchmaking user 2 login succeeded." : "failed; " + response.Message;
			});		
		}

		public void realtimeConnect() {
			
			BacktoryClient.StartRealtimeService (new BacktoryConnectionStatusListener() {
				OnOpen = () => {
					MessageText.text = "Realtime service started successfully!";
				},
				OnClose = () => {
					MessageText.text = "Realtime service stopped successfully!";
				},
				OnError = (message) => {
					MessageText.text = "Start realtime error!" + message;
				}
			});
			BacktoryClient.SetOnErrorListener ((errorMsg) => {
				MessageText.text = JsonConvert.SerializeObject (errorMsg, Formatting.Indented, JsonnetSetting ());
			});
		}

		public void realtimeDisconnect() {
			BacktoryClient.StopRealtimeService ();
		}

		public void requestMatch() {
			mm = new BacktoryMatchMaking(categoryInput.text, int.Parse(skillInput.text))
			{
				OnMatchFound = (match) =>
				{
					MessageText.text = "Match found!\n" +
					                   JsonConvert.SerializeObject(match, Formatting.Indented, JsonnetSetting());
					setupRealtimeGame(match);
				},
				OnMatchmakingUpdate = (mmUpdateMessage) =>
				{
					MessageText.text = "Matchmaking update received!\n" +
					                   JsonConvert.SerializeObject(mmUpdateMessage, Formatting.Indented, JsonnetSetting());
				},
				OnMatchNotFound = (message) =>
				{
					MessageText.text = "Match not found!\n" +
					                   JsonConvert.SerializeObject(message, Formatting.Indented, JsonnetSetting());
				}
			};

			mm.Request ("metaData", (response) => {
				ResultText.text = response.Successful ? "Request match succeeded." : "failed; " + response.Message;
			});
		}

		public void cancelMatchRequest() {
			if (mm == null) {
				ResultText.text = "No matchmaking available!";
				return;
			}				
			mm.Cancel ((response) => {
				ResultText.text = response.Successful ? "Canceling request match succeeded." :
					"failed; " + response.Message;
			});
		}




		/* 
		 * Challenge
		 */
		public void loginChallengeUser1() {
			BacktoryUser.LoginInBackground(testUser1.username, testUser1.password, (IBacktoryResponse response) => {
				ResultText.text = response.Successful ? "Challenge user 1 login succeeded." : "failed; " + response.Message;
				setChallengeListeners();
			});
		}

		public void loginChallengeUser2() {
			BacktoryUser.LoginInBackground(testUser2.username, testUser2.password, (IBacktoryResponse response) => {
				ResultText.text = response.Successful ? "Challenge user 2 login succeeded." : "failed; " + response.Message;
				setChallengeListeners();
			});		
		}

		BacktoryChallenge invitedChallenge;

		private void setChallengeListeners() {
			BacktoryChallenge.SetOnChallengeInvitationListener((challenge) => {
				MessageText.text = "Invited to a challenge!\n" + 
					JsonConvert.SerializeObject (challenge, Formatting.Indented, JsonnetSetting ());
				invitedChallenge = challenge;
			});
			BacktoryChallenge.SetOnChallengeFailedListener((message) => {
				MessageText.text = "Challenge failed!\n" + 
					JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				invitedChallenge = null;
			});
			BacktoryChallenge.SetOnChallengeAcceptedListener((message) => {
				MessageText.text = "Challenge accepted!\n" + 
					JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
			});
			BacktoryChallenge.SetOnChallengeRejectedListener((message) => {
				MessageText.text = "Challenge rejected!\n" + 
					JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
			});
			BacktoryChallenge.SetOnChallengeReadyListener((match) => {
				MessageText.text = "Challenge is ready!\n" + 
					JsonConvert.SerializeObject (match, Formatting.Indented, JsonnetSetting ());
				setupRealtimeGame(match);
			});
		}

		BacktoryChallenge requestedChallenge;

		public void requestChallenge() {
			if (BacktoryUser.CurrentUser == null) {
				ResultText.text = "Not logged in yet!";
				return;
			}

			IList<string> challengedUsers = new List<string>();
			if (BacktoryUser.CurrentUser.Username == testUser1.username) {
				challengedUsers.Add (testUser2.userId);
			} else if (BacktoryUser.CurrentUser.Username == testUser2.username) {
				challengedUsers.Add (testUser1.userId);
			}

			BacktoryChallenge.CreateNew(challengedUsers, 2, 25, (response) => {
				if (response.Successful) {
					ResultText.text = "New challenge requested!\n" + JsonConvert.SerializeObject (response.Body, Formatting.Indented, JsonnetSetting ());
					requestedChallenge = response.Body;
				} else {
					ResultText.text = response.Message;
				}
			}, "challengeName", "metaData");
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
			invitedChallenge.Accept ("metaData", (response) => {
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
			BacktoryChallenge.ActiveChallenges (PrintCallBack<IList<BacktoryChallenge>>());
		}




		/* 
		 * Realtime
		 */
		BacktoryRealtimeGame realtimeGame;

		private void setupRealtimeGame(BacktoryMatch match)
		{
			this.GetComponent<UIController> ().enableRealtimeModule ();

			realtimeGame = new BacktoryRealtimeGame(match)
			{
				OnGameEvent = (message) =>
				{
					MessageText.text = "Game event received!\n" +
					                   JsonConvert.SerializeObject(message, Formatting.Indented, JsonnetSetting());
				},
				OnDirectMessage = (message) =>
				{
					MessageText.text = "Direct chat received.\n" +
					                   JsonConvert.SerializeObject(message, Formatting.Indented, JsonnetSetting());
				},
				OnError = (message) =>
				{
					MessageText.text = "Error occurred!\n" +
					                   JsonConvert.SerializeObject(message, Formatting.Indented, JsonnetSetting());
				},
				OnGameEnded = (message) =>
				{
					MessageText.text = "Game ended!\n" +
					                   JsonConvert.SerializeObject(message, Formatting.Indented, JsonnetSetting());
				},
				OnGameStarted = () => { MessageText.text = "Game started !!!!!!!!!!"; },
				OnGameStartedWebhook = (message) => { Debug.Log("Your start webhook returned this message: " + message); },
				OnPlayerJoined = (message) =>
				{
					MessageText.text = "Player joined the game!\n" +
					                   JsonConvert.SerializeObject(message, Formatting.Indented, JsonnetSetting());
				},
				OnPlayerJoinedWebhook = (message) => { Debug.Log("Your join webhook sent this message to you: " + message); },
				OnPlayerLeft = (message) =>
				{
					MessageText.text = "Player left the game!\n" +
					                   JsonConvert.SerializeObject(message, Formatting.Indented, JsonnetSetting());
				},
				OnPublicMessage = (message) =>
				{
					MessageText.text = "Public message received!\n" +
					                   JsonConvert.SerializeObject(message, Formatting.Indented, JsonnetSetting());
				},
				OnServerMessage = (message) =>
				{
					MessageText.text = "Server message received!\n" +
					                   JsonConvert.SerializeObject(message, Formatting.Indented, JsonnetSetting());
				},
				OnWebHookError = (message) => { Debug.Log("Error in server webhook: " + message); }
			};
		}

		public void joinGame()
		{
			if (realtimeGame == null) {
				ResultText.text = "No game is available.";
				return;
			}
			realtimeGame.Join ();
		}

		public void sendEvent()
		{
			if (realtimeGame == null) {
				ResultText.text = "No game is available.";
				return;
			}
			Dictionary<string, string> data = new Dictionary<string, string>();
			realtimeGame.SendGameEvent(messageInput.text, data);
		}

		public void directMessage()
		{
			if (realtimeGame == null) {
				ResultText.text = "No game is available.";
				return;
			}
			if (BacktoryUser.CurrentUser == null) {
				ResultText.text = "Not logged in yet!";
				return;
			}

			string contactUserId = "";
			if (BacktoryUser.CurrentUser.Username == testUser1.username) {
				contactUserId = testUser2.userId;
			} else if (BacktoryUser.CurrentUser.Username == testUser2.username) {
				contactUserId = testUser1.userId;
			}

			realtimeGame.SendDirectMessage(contactUserId,
				messageInput.text, (response) => {
				ResultText.text = response.Successful ? "Send direct chat in match succeeded." : "failed; " + response.Message;
			});
		}

		public void sendChatToMatch()
		{
			if (realtimeGame == null) {
				ResultText.text = "No game is available.";
				return;
			}
			realtimeGame.SendPublicMessage (messageInput.text, (response) => {
				ResultText.text = response.Successful ? "Send public message in match succeeded." : "failed; " + response.Message;
			});
		}

		public void sendMatchResult() 
		{
			if (realtimeGame == null) {
				ResultText.text = "No game is available.";
				return;
			}
			IList<string> winners = new List<string>{ testUser1.userId };
			realtimeGame.SendWinners (winners, "extraData", (response) => {
				ResultText.text = response.Successful ? "Send winners in match succeeded." : "failed; " + response.Message;
			});
		}

		public void leaveGame() 
		{
			if (realtimeGame == null) {
				ResultText.text = "No game is available.";
				return;
			}
			realtimeGame.Leave ();
			realtimeGame = null;
			this.GetComponent<UIController> ().disableRealtimeModule ();
		}


		/* 
		 * Person-to-Person Chat 
		 */
		public void loginChatUser()
		{
			BacktoryUser.LoginInBackground(testUser1.username, testUser1.password, (IBacktoryResponse response) => {
				ResultText.text = response.Successful ? "Chat user login succeeded" : "failed; " + response.Message;
				BacktoryChat.Direct.SetOnReceivingMessageListener ((message) => {
					MessageText.text = JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				});
				BacktoryChat.Group.SetOnInvitingToJoinListener((message) => {
					MessageText.text = "Invited to join!\n" + 
						JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
					invitedGroupId = message.GroupId;
				});
				BacktoryChat.Group.SetOnMemberAddedListener((message) => {
					MessageText.text = "Member added to group!\n" + 
						JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				});
				BacktoryChat.Group.SetOnMemberJoinedListener((message) => {
					MessageText.text = "Member joined the group!\n" + 
						JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				});
				BacktoryChat.Group.SetOnMemberLeftListener((message) => {
					MessageText.text = "Member left the group!\n" + 
						JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				});
				BacktoryChat.Group.SetOnMemberRemovedListener((message) => {
					MessageText.text = "Member is removed from group!\n" + 
						JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				});
				BacktoryChat.Group.SetOnReceivingMessageListener((message) => {
					MessageText.text = "Message received from group chat!\n" + 
						JsonConvert.SerializeObject (message, Formatting.Indented, JsonnetSetting ());
				});
			});
		}
			
		public void sendChatMessage()
		{
			int id = UnityEngine.Random.Range (0, 10);
			BacktoryChat.Direct dc = new BacktoryChat.Direct (testUser1.userId);
			dc.SendMessage ("Working! " + id, (response) => {
				ResultText.text = response.Successful ? "Sending chat succeeded" : "failed; " + response.Message;
			});
		}

		public void requestOfflineChats()
		{
			BacktoryChat.OfflineMessages (PrintCallBack<IList<AbsChatMessage>>());
		}

		public static long CurrentTimeMillis()
		{
			DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (long) (DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
		}

		public void requestChatHistory ()
		{
			BacktoryChat.Direct dc = new BacktoryChat.Direct (testUser2.userId);
			dc.History (CurrentTimeMillis(), PrintCallBack<IList<BacktoryDirectChatMessage>>());
		}


		/* 
		 * Group Chat 
		 */
		private string myGroupId = "58c1a989e4b07d84e2ebde2d";

		public void createChatGroup() 
		{
			int id = UnityEngine.Random.Range (0, 10);
			BacktoryChat.Group.CreateNewGroup ("MyGroup" + id, BacktoryChat.Group.Mode.Private,
				PrintCallBack<BacktoryChat.Group>());
			
		}

		public void requestGroupsList()
		{
			BacktoryChat.Group.MyGroups ((response) => {
				if (response.Successful)	
					ResultText.text = JsonConvert.SerializeObject (response.Body, Formatting.Indented, JsonnetSetting ());
				else
					ResultText.text = response.Message;
			});
		}

		public void requestMembersList()
		{
			var bgc = new BacktoryChat.Group (myGroupId);
			bgc.MembersInfo (PrintCallBack<IList<BacktoryGroupMemberInfo>>());
		}

		public void addGroupMember()
		{
			var bgc = new BacktoryChat.Group (myGroupId);
			bgc.AddMember (testUser1.userId, (response) => {
				ResultText.text = response.Successful ? "Add group member succeeded." : "failed; " + response.Message;
			});	
		}

		public void removeGroupMember()
		{
			var bgc = new BacktoryChat.Group (myGroupId);
			bgc.RemoveMember (testUser1.userId, (response) => {
				ResultText.text = response.Successful ? "Remove group member succeeded." : "failed; " + response.Message;
			});
		}

		public void sendChatToGroup() 
		{
			var bgc = new BacktoryChat.Group (myGroupId);
			bgc.SendMessage ("Hello Everybody!", (response) => {
				ResultText.text = response.Successful ? "Send chat to group succeeded." : "failed; " + response.Message;
			});
		}

		public void requestGroupChatHistory()
		{
			var bgc = new BacktoryChat.Group (myGroupId);
			bgc.History(CurrentTimeMillis(), PrintCallBack<IList<AbsGroupChatMessage>>());
		}

		string invitedGroupId;

		public void inviteToGroup() 
		{
			var bgc = new BacktoryChat.Group (myGroupId);
			bgc.InviteToGroup (testUser1.userId,  (response) => {
				ResultText.text = response.Successful ? "Invitation to group chat succeeded." : "failed; " + response.Message;
			});
		}

		public void joinGroup()
		{
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

		public void leaveGroup()
		{
			var bgc = new BacktoryChat.Group (myGroupId);
			bgc.LeaveGroup ((response) => {
				ResultText.text = response.Successful ? "Left the group chat successfully!" : "failed; " + response.Message;
			});
		}

		public void makeMemberOwner()
		{
			var bgc = new BacktoryChat.Group (myGroupId);
			bgc.MakeMemberOwner (testUser1.userId, (response) => {
				ResultText.text = response.Successful ? "Member is owner now!" : "failed; " + response.Message;
			});
		}


		
		
		
		/* 
		 * CafeBazaar In-app Purchase 
		 */
		private BacktoryIap _backtoryIap;

		internal class BacktoryIapListenerImpl : IBacktoryIapListener
		{
			private readonly Sample _sample;
			
			public BacktoryIapListenerImpl(Sample sample)
			{
				_sample = sample;
			}
			
			public void OnGetSkuDetailsFinished(IapResult result, IList<SkuDetails> skuDetailsList)
			{
				_sample.ResultText.text = JsonConvert.SerializeObject
					(result, Formatting.Indented, JsonnetSetting ()) + "\n" + 
				                   JsonConvert.SerializeObject
					                   (skuDetailsList, Formatting.Indented, JsonnetSetting ());
			}

			public void OnGetPurchasesFinished(IapResult result, IList<string> ownedSkus, string continuationToken)
			{
				_sample.ResultText.text = JsonConvert.SerializeObject
					                   (result, Formatting.Indented, JsonnetSetting ()) + "\n" + 
				                   JsonConvert.SerializeObject
					                   (ownedSkus, Formatting.Indented, JsonnetSetting ()) + "\n" + 
									"continuationToken: " + continuationToken;				
			}

			public void OnPurchaseFinished(IapResult result, Purchase purchase, string webhookMessage)
			{
				_sample.ResultText.text = JsonConvert.SerializeObject
					                          (result, Formatting.Indented, JsonnetSetting ()) + "\n" + 
				                          JsonConvert.SerializeObject
					                          (purchase, Formatting.Indented, JsonnetSetting ()) + "\n" +
																	"webhookMessage: " + webhookMessage;
				if (result.ResultCode == IapResult.SUCCESS)
				{
					_sample.PurchaseToken = purchase.PurchaseToken;
				}
			}

			public void OnConsumptionFinished(IapResult result, string sku, string purchaseToken, string webhookMessage)
			{
				_sample.ResultText.text = JsonConvert.SerializeObject
					                          (result, Formatting.Indented, JsonnetSetting()) + "\n" +
				                          "sku: " + sku + "\n" +
				                          "purchaseToken: " + purchaseToken + "\n" +
																	"webhookMessage: " + webhookMessage;
				if (result.ResultCode == IapResult.SUCCESS)
				{
					_sample.PurchaseToken = null;
				}
			}

			public void OnSubscriptionFinished(IapResult result, Purchase purchase, string webhookMessage)
			{
				_sample.ResultText.text = JsonConvert.SerializeObject
					                          (result, Formatting.Indented, JsonnetSetting ()) + "\n" + 
				                          JsonConvert.SerializeObject
					                          (purchase, Formatting.Indented, JsonnetSetting ()) + "\n" +
				                          "webhookMessage: " + webhookMessage;
			}
		}
		
		private void CreateBacktoryIapCoreInstance()
		{
			Debug.Log("Creating backtory iap core instance ...");
			var iapBehaviour = GameObject.Find("BacktoryInitializeBahaviour")
				.GetComponent<BacktoryIapBehaviour>();
			_backtoryIap = new BacktoryIap(iapBehaviour,
				new BacktoryIapListenerImpl(this), "ir.pegahtech.backtory.sdksample");
		}
		
		
		public void GetSkuDetails()
		{
			if (_backtoryIap == null)
			{
				CreateBacktoryIapCoreInstance();
			}

			var skuList = new List<string> {"gas"}; 
			_backtoryIap.GetSkuDetailsInBackground(skuList);
		}

		public void GetPurchases()
		{
			if (_backtoryIap == null)
			{
				CreateBacktoryIapCoreInstance();
			}
			
			var bazaarItemType = ItemTypesDropdown.captionText.text;
			_backtoryIap.GetPurchases(bazaarItemType, null);
		}

		public string PurchaseToken { get; set; }

		public void PurchaseItem()
		{
			if (_backtoryIap == null)
			{
				CreateBacktoryIapCoreInstance();
			}

			var securityType = SecurityTypesDropdown.value;
			if (securityType == 0) 	// secure
			{
				_backtoryIap.SecurePurchase("gas", "Nothing to say.");
			}
			else 					// insecure
			{
				_backtoryIap.InsecurePurchase("gas");
			}
			
		}

		public void ConsumeItem()
		{
			if (_backtoryIap == null)
			{
				CreateBacktoryIapCoreInstance();
			}

			if (PurchaseToken == null)
			{
				ResultText.text = "No purchaseToken is available!";
				return;
			}
			
			var securityType = SecurityTypesDropdown.value;
			if (securityType == 0)
			{
				_backtoryIap.SecureConsume("gas", PurchaseToken, "Nothing to say.");
			}
			else
			{
				_backtoryIap.InsecureConsume("gas", PurchaseToken);
			}
			
		}

		public void UpgradeToPremium()
		{
			if (_backtoryIap == null)
			{
				CreateBacktoryIapCoreInstance();
			}
			
			var securityType = SecurityTypesDropdown.value;
			if (securityType == 0)
			{
				_backtoryIap.SecurePurchase("premium", "Nothing to say.");
			}
			else
			{
				_backtoryIap.InsecurePurchase("premium");	
			}
		}

		public void Subscribe()
		{
			if (_backtoryIap == null)
			{
				CreateBacktoryIapCoreInstance();
			}
			
			var securityType = SecurityTypesDropdown.value;
			if (securityType == 0)
			{
				_backtoryIap.SecureSubscribe("infinite_gas", "Nothing to say.");
			}
			else
			{
				_backtoryIap.InsecureSubscribe("infinite_gas");
			}
		}
		#endregion

		
		
		
		
		#region sample stuff

		private class TestUser
		{
			public string name;
			public string family;
			public string username;
			public string password;
			public string email;
			public string userId;
		}
						
		private TestUser testUser1 = new TestUser {
			name = "TestUser",
			family = "-",
			username = "testUser",
			password = "12341234",
			email = "mm49307@gmail.com",
			userId = "593d6888e4b0c8960312d7b4"
		};
				
		private static TestUser testUser2 = new TestUser {
			name = "TestUser2",
			family = "-",
			username = "testUser2",
			password = "12341234",
			email = "mm493072@gmail.com",
			userId = "593d68c5e4b044a0bab405aa"
		};

		internal Action<IBacktoryResponse<T>> PrintCallBack<T> ()
		{
			return (backtoryResponse) => {
				if (backtoryResponse.Successful)
					ResultText.text = JsonConvert.SerializeObject (backtoryResponse.Body, Formatting.Indented, JsonnetSetting ());
				else
					ResultText.text = "failed; " + backtoryResponse.Message;
			};
		}

		private static JsonSerializerSettings JsonnetSetting ()
		{
			return new JsonSerializerSettings () {
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore,
				DefaultValueHandling = DefaultValueHandling.Include,
				ContractResolver = new CamelCasePropertyNamesContractResolver (),
			};
		}
		#endregion
	}
}
