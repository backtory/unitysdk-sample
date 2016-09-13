using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.BacktorySDK.core
{
    /// <summary>
    /// A place for handling SDK-scoped events. A typical usage could be redirecting user to login scene/page 
    /// on receiving logout event.
    /// <para>Implement this interface and set an instance of it on <see cref="BacktoryManager.GlobalEventListener"/>
    /// </summary>
    public interface IGlobalEventListener
    {
        void OnEvent(BacktorySDKEvent logoutEvent);
    }

}
