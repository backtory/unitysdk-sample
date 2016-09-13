using Assets.BacktorySDK.attributes;
using Assets.BacktorySDK.auth;
using Assets.BacktorySDK.core;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.BacktorySDK.game
{
    public class BacktoryGameEvent
    {
        // names are matched with json entities on server side. Provide @SerializedName annotation in
        // case you want to change their name.
        [JsonProperty(PropertyName = "eventName")]
        public string Name { internal set; get; }

        public IList<FieldValue> FieldsAndValues { get; internal set; }

        public BacktoryGameEvent()
        {
            // getting one and only static property/field annotated with EventNameAttribute attribute
            var nameField = Utils.GetFieldByAttribute(typeof(EventNameAttribute), this, true);
            if (nameField != null)
            {
                Name = nameField.GetValue(null) as string;
            }
            else { // try with properties
                var nameProp = Utils.GetPropertyByAttribute(typeof(EventNameAttribute), this, true);

                if (nameProp != null)
                    Name = nameProp.GetValue(null, null) as string;
            }
        }


        private IRestRequest SendEventRequest(BacktoryGameEvent backtoryGameEvent)
        {
            var request = Backtory.RestRequest("game/events", Method.POST);
            request.AddHeader(Backtory.GameInstanceIdString, BacktoryConfig.BacktoryGameInstanceId);
            request.AddHeader(BacktoryUser.KeyAuthorization, BacktoryUser.AuthorizationHeader());
            request.AddHeader("Accept", Backtory.ApplicationJson);
            request.AddJsonBody(backtoryGameEvent);

            return request;
        }

        /// <summary>
        /// Synchronously sends this event to update its corresponding leaderboard and returns its response.
        /// <para>Typically, you should use {@link #sendAsync(BacktoryCallBack)} instead of this, unless
        /// you are managing your own threading.</para>
        /// </summary>
        /// <returns>Backtory user registered on server wrapped in a <see cref="BacktoryResponse{T}"/>.</returns>
        /// <seealso cref="BacktoryLeaderBoard"/>
        public BacktoryResponse<object> send()
        {
            InitFieldsUsingAnnotations(this);
            // TODO: some precondition check e.g. values are numbers
            return Backtory.ExecuteRequest<object>(SendEventRequest(this));
        }

        /// <summary>
        /// Sends this event to update its corresponding leaderboard in background.
        /// <para>This is preferable to using <see cref="send"/>, unless your code is already running from a
        /// background thread.</para>
        /// </summary>
        /// <param name="callBack">callback notified upon receiving server response or any error in the
        /// process. Server response is a Backtory user registered on server wrapped in a <see cref="BacktoryResponse{T}"/>.
        /// Server response contains no body.</param>
        /// <seealso cref="BacktoryLeaderBoard"/>
        public void sendAsync(Action<BacktoryResponse<object>> callBack)
        {
            InitFieldsUsingAnnotations(this);
            Backtory.ExecuteRequestAsync(SendEventRequest(this), callBack);
        }

        /// <summary>
        /// Reads annotation fields for event name and field-values and set them to
        /// proper event fields. After this method, json serialization of event will meet server expectation
        /// </summary>
        /// <param name="gameEvent">always is caller event. Just for test purposes</param>
        private void InitFieldsUsingAnnotations(BacktoryGameEvent gameEvent)
        {
            var temp = new List<FieldValue>();
            var fieldNamesProps = Utils.GetPropertiesByAttribute(typeof(FieldNameAttribute), this, true);
            foreach (var prop in fieldNamesProps)
            {
                FieldNameAttribute attr = prop.GetCustomAttributes(typeof(FieldNameAttribute), true).Single() as FieldNameAttribute;
                if (attr == null) throw new InvalidOperationException("something bad in reflection");

                var val = prop.GetValue(this, null);
                if (val is int)
                    temp.Add(new FieldValue(attr.Name, (int)val));
                else 
                    throw new ArgumentException("\"Fields\"s can only accept integers as their values. Your's is " + val.GetType());
                
            }
            FieldsAndValues = temp;
        }


#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
        public class FieldValue
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
        {
            public string FieldName { get; internal set; }
            public int Value { get; internal set; }

            public FieldValue(string fieldName, int value)
            {
                FieldName = fieldName;
                Value = value;
            }

            // override object.Equals
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                FieldValue that = (FieldValue)obj;

                return Value == that.Value &&
                    (FieldName != null ? FieldName.Equals(that.FieldName) : that.FieldName == null);
            }
        }
    }
}
