using Assets.BacktorySDK.attributes;
using Assets.BacktorySDK.auth;
using Assets.BacktorySDK.core;
using RestSharp;
using System;
using System.Collections.Generic;
namespace Assets.BacktorySDK.game
{
    /// <summary>
    /// Class representation of Backtory leaderboard entity. Each leaderboard is distinguished from others
    /// by an Id accessible from backtory panel.
    /// <para>A leaderboard can be used to get info about leaderboard players and position of user in leaderboard</para>
    /// <para>All methods of leaderboard class which perform network call needs that 
    /// <see cref="leaderBoardId"/> to has been set. See examples in class documentation to see usage methods.
    /// </summary>
    /// <example> setting <see cref="LeaderboardId"/> not using attribute
    /// <code>
    ///  LeaderBoard topPlayers = new LeaderBoard();
    ///  topPlayers.setLeaderBoardId("leaderboard id from panel");
    ///  topPlayers.getTopPlayersInBackground(5, callback)
    /// </code>
    /// <example>Extending leaderBoard class and annotating a static field as leaderBoardId for all instances of this
    /// extended leaderboard</example>
    /// <code>
    ///  public class TopPlayersLeaderBoard extends BacktoryLeaderBoard {
    /// 
    ///    LeaderBoardId
    ///    public static final string id = "leaderboard id";
    ///  }
    ///  ...
    ///  new TopPlayersLeaderBoard().getTopPlayersInBackground(5, callback);
    /// </code>
    /// </example>

    public class BacktoryLeaderBoard
    {
        public string LeaderboardId { set; get; }

        public BacktoryLeaderBoard()
        {
            // getting one and only static property/field annotated with EventNameAttribute attribute
            var leaderboardField = Utils.GetFieldByAttribute(typeof(LeaderboardIdAttribute), this, true);
            if (leaderboardField != null)
            {
                LeaderboardId = leaderboardField.GetValue(null) as string;
            }
            else
            { // try with properties
                var leaderboardProp = Utils.GetPropertyByAttribute(typeof(EventNameAttribute), this, true);

                if (leaderboardProp != null)
                    LeaderboardId = leaderboardProp.GetValue(null, null) as string;
            }
        }

        #region services

        private IRestRequest AddDefaultHeaders(IRestRequest rawRequest)
        {
            rawRequest.AddHeader(Backtory.GameInstanceIdString, BacktoryConfig.BacktoryGameInstanceId);
            rawRequest.AddHeader(BacktoryUser.KeyAuthorization, BacktoryUser.AuthorizationHeader());
            rawRequest.AddHeader("Accept", Backtory.ApplicationJson);
            return rawRequest;
        }

        #region player rank

        private IRestRequest PlayerRankRequest()
        {
            var request = Backtory.RestRequest("game/leaderboards/{leader_board_id}", Method.GET);
            request = AddDefaultHeaders(request);
            request.AddParameter("leader_board_id", LeaderboardId, ParameterType.UrlSegment);
            return request;
        } 
        /// <summary>
        /// Synchronously gets the current user position in this leaderboard and returns its response.
        /// </summary>
        /// <returns>a LeaderBoardRank object which presents current user rank in this leaderboard wrapped
        /// in a <see cref="BacktoryResponse{T}"/></returns>
        /// <seealso cref="LeaderBoardRank"/>
        public BacktoryResponse<LeaderBoardRank> GetPlayerRank()
        {
            return Backtory.ExecuteRequest<LeaderBoardRank>(PlayerRankRequest());
        }

        /// <summary>
        /// Gets the current user position in this leaderboard in background.
        /// </summary>
        /// <param name="backtoryCallBack">backtoryCallBack callback notified upon receiving server response or any error in the
        /// process. Server response is a LeaderBoardRank object which presents current user position in
        /// leaderboard wrapped in a <see cref="BacktoryResponse{T}"/></param> 
        /// <seealso cref="LeaderBoardRank"/> 
        public void GetPlayerRankInBackground(Action<BacktoryResponse<LeaderBoardRank>> backtoryCallBack)
        {
            Backtory.ExecuteRequestAsync(PlayerRankRequest(), backtoryCallBack);
        }
        #endregion

        #region top players
        private IRestRequest TopPlayersRequest(int count)
        {
            var request = Backtory.RestRequest("game/leaderboards/top/{leader_board_id}", Method.GET);
            request = AddDefaultHeaders(request);
            request.AddParameter("leader_board_id", LeaderboardId, ParameterType.UrlSegment);
            request.AddQueryParameter("count", count.ToString());
            return request;
        }

        /// <summary>
        /// Synchronously gets top players of this leaderboard.
        /// </summary>
        /// <param name="count">count number of players in top of leaderboard you want to receive. Limited to 100.</param> 
        /// <returns>a LeaderBoardResponse object presenting players info wrapped in a <see cref="BacktoryResponse{T}"/></returns> 
        public BacktoryResponse<LeaderBoardResponse> GetTopPlayers(int count)
        {
            return Backtory.ExecuteRequest<LeaderBoardResponse>(TopPlayersRequest(count));
        }

        /// <summary>
        /// Gets top players of this leaderboard in background.
        /// </summary>
        /// <param name="count"></param> number of players in top of leaderboard you want to receive. Limited to 100.
        /// <param name="backtoryCallBack"></param> backtoryCallBack callback notified upon receiving server response or any error in the
        /// process. Server response is a a LeaderBoardResponse object presenting players info
        /// wrapped in a {@link BacktoryResponse}
        /// </summary>
        public void GetTopPlayersInBackground(int count, Action<BacktoryResponse<LeaderBoardResponse>> backtoryCallBack)
        {
            Backtory.ExecuteRequestAsync(TopPlayersRequest(count), backtoryCallBack);
        }
        #endregion

        #region around me

        private IRestRequest AroundMeRequest(int count)
        {
            var request = Backtory.RestRequest("game/leaderboards/around-me/{leader_board_id}", Method.GET);
            request = AddDefaultHeaders(request);
            request.AddParameter("leader_board_id", LeaderboardId, ParameterType.UrlSegment);
            request.AddQueryParameter("count", count.ToString());
            return request;
        }

        /// <summary>
        /// Synchronously gets players in leaderboard who are around of current user.
        /// </summary>
        /// <param name="count">number of players around of current user you want to receive. Half of this number
        ///              would be on top of current user and half would be under. Limited to 20.</param> 
        /// <returns> callback notified upon receiving server response or any error in the
        /// process. Server response is a a LeaderBoardResponse object presenting players info
        /// wrapped in a <see cref="BacktoryResponse{T}"/></returns>
        public BacktoryResponse<LeaderBoardResponse> GetPlayersAroundMe(int count)
        {
            return Backtory.ExecuteRequest<LeaderBoardResponse>(AroundMeRequest(count));
        }

        /// <summary>
        /// Gets players in leaderboard who are around of current user in background.
        /// </summary>
        /// <param name="count">number of players around of current user you want to receive. Half of this number
        ///              would be on top of current user and half would be under. Limited to 20.</param>
        /// <param name="backtoryCallBack"> callback notified upon receiving server response or any error in the
        /// process. Server response is a a LeaderBoardResponse object presenting players info
        /// wrapped in a <see cref="BacktoryResponse{T}"/></param>
        public void GetPlayersAroundMeInBackground(int count,
                                                   Action<BacktoryResponse<LeaderBoardResponse>> backtoryCallBack)
        {
            Backtory.ExecuteRequestAsync(AroundMeRequest(count), backtoryCallBack);
        }

        #endregion

        #endregion

        #region inner classes
        
        /// <summary>
        /// Contains a number of players info and score from a leaderboard
        /// </summary>
        public class LeaderBoardResponse
        {
            public List<UserProfile> UsersProfile { get; internal set; }
            public string Message { get; internal set; }
        }

        /// <summary>
        /// Information about a player's rank in a leaderboard and scores he's gained on different fields.
        /// </summary>
        public class LeaderBoardRank
        {
            public int Rank { get; internal set; }
            public List<int> Scores { get; internal set; }
        }

        /// <summary>
        /// Contains personal information and of a player in this leaderboard along with scores he's
        /// gained from different fields.
        /// </summary>
        public class UserProfile
        {
            public UserBriefProfile UserBriefProfile { get; internal set; }
            public List<int> Scores { get; internal set; }
        }

        public class UserBriefProfile
        {
            public string FirstName { get; internal set; }
            public string UserName { get; internal set; }
            public string UserId { get; internal set; }

        }

        #endregion
    }

}