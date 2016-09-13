namespace Assets.BacktorySDK
{
    public abstract class BacktorySDKEvent
    {
        internal static BacktorySDKEvent LogoutEvent()
        {
            return new LogoutEvent();
        }
    }

    public class LogoutEvent : BacktorySDKEvent { }
    
}
