using System;
using Assets.BacktorySDK.game;

namespace Assets.BacktorySDK.attributes
{
    /// <summary>
    /// Denotes the <i>fieldName</i> of a <see cref="BacktoryGameEvent.FieldValue"/>
    /// which this value must assigned to. Must be used for fields of an <c>BacktoryEvent</c>
    /// object. 
    /// <para>See example part in XML doc for sample usage</para>
    /// </summary>
    /// <code>
    /// public class GameOverEvent : BacktoryEvent
    /// {
    ///     [EventName]
    ///     public const string eventName = "GameOver";
    ///     
    ///     [FieldNameAttribute("Coin")]
    ///     int coinValue;
    ///
    ///     [FieldNameAttribute("Time")]
    ///     int timeValue;
    ///     .
    ///     .
    ///     .
    ///     public GameOverEvent(int coinValue, int timeValue)
    ///     {
    ///         CoinValue = coinValue;
    ///         TimeValue = timeValue;
    ///     }
    /// </code>
    /// <seealso cref="EventNameAttribute"/>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FieldNameAttribute : Attribute
    {
        public readonly string Name;
        public FieldNameAttribute(string fieldName)
        {
            Name = fieldName;
        }
    }
}
