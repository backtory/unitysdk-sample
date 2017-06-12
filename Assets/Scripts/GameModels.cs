using Backtory.Core.Public;

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
        public static string id = "5836f966e4b02f23879ace90";
    }
}
