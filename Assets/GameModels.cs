using Assets.BacktorySDK.attributes;
using Assets.BacktorySDK.game;

namespace Assets
{
    public class GameOverEvent : BacktoryGameEvent
    {

        [EventName]
        public static string eventName = "GameOver";

        [FieldName("Coin")]
        public int CoinValue { set; get; }

        [FieldName("Time")]
        public int TimeValue { set; get; }

        public GameOverEvent(int coinValue, int timeValue)
        {
            CoinValue = coinValue;
            TimeValue = timeValue;
        }
    }

    public class TopPlayersLeaderBoard : BacktoryLeaderBoard
    {
        [LeaderboardId]
        public static string id = "576d4a33e4b050913524162c";
    }
}
