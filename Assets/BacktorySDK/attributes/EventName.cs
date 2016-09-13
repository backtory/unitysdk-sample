using System;
using Assets.BacktorySDK.game;

namespace Assets.BacktorySDK.attributes
{
    /// <summary>
    /// Denotes the name for <see cref="BacktoryGameEvent"/> object.
    /// can be set on <i>static</i> fields and properties.
    /// <para>See the example part in XML doc for sample usage</para>
    /// </summary>
    /// <example><code>
    /// public class GameOverEvent : BacktoryEvent
    /// {
    ///     [EventNameAttribute]
    ///     public const string eventName = "GameOver";
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="FieldNameAttribute"/>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    class EventNameAttribute : Attribute
    {
    }
}
