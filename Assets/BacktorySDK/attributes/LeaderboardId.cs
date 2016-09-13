using System;
using Assets.BacktorySDK.game;

namespace Assets.BacktorySDK.attributes
{
    /// <summary>
    /// Use this annotation to denote the its target for using as <see cref="BacktoryLeaderBoard"/> id attribute.
    /// Can be used on <i>static</i> fields and properties.
    /// <para>see example part in XML doc for sample usage</para>
    /// </summary>
    /// <example><code>
    /// public class BestRecordsLeaderboard : BacktoryLeaderboard
    /// {
    ///     [LeaderboardIdAttribute]
    ///     public const string id = "576d4a33e4b050913524162c";
    /// }
    /// </code></example>
    /// <seealso cref="BacktoryLeaderBoard"/>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    class LeaderboardIdAttribute : Attribute
    {
    }
}
