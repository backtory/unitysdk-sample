using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using JsonNet = Newtonsoft.Json.JsonSerializer;
using System.IO;
using RestSharp.Deserializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Assets.BacktorySDK.core
{
    #region serialization strategy
    //internal class SnakeJsonSerializerStrategy : PocoJsonSerializerStrategy
    //{
    //    protected override string MapClrMemberNameToJsonFieldName(string clrPropertyName)
    //    {
    //        var rest = new RestClient();
    //        //PascalCase to snake_case
    //        return string.Concat(clrPropertyName.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + char.ToLower(x).ToString() : x.ToString()));
    //    }
    //}
    #endregion

    // just for calling MySimpleJson instead of RestSharp Simple Json in serializa method of "JsonSerializer" class
    //internal class MyJsonSerializer : JsonSerializer
    //{
    //    public new string Serialize(object obj)
    //    {
    //        Debug.Log("wtf wtf wtf");
    //        return MySimpleJson.SerializeObject(obj);
    //    }
    //}


    //#region Custom SimpleJson (handling serializedAs attributes)
    ///// <summary>
    ///// Default JSON serializer for request bodies
    ///// Doesn't currently use the SerializeAs attribute, defers to Newtonsoft's attributes
    ///// </summary>
    //public class MyJsonSerializer : ISerializer
    //{
    //    /// <summary>
    //    /// Default serializer
    //    /// </summary>
    //    public MyJsonSerializer()
    //    {
    //        this.ContentType = "application/json";
    //    }

    //    /// <summary>
    //    /// Serialize the object as JSON
    //    /// </summary>
    //    /// <param name="obj">Object to serialize</param>
    //    /// <returns>JSON as String</returns>
    //    public virtual string Serialize(object obj)
    //    {
    //        return MySimpleJson.SerializeObject(obj);
    //    }

    //    /// <summary>
    //    /// Unused for JSON Serialization
    //    /// </summary>
    //    public string DateFormat { get; set; }

    //    /// <summary>
    //    /// Unused for JSON Serialization
    //    /// </summary>
    //    public string RootElement { get; set; }

    //    /// <summary>
    //    /// Unused for JSON Serialization
    //    /// </summary>
    //    public string Namespace { get; set; }

    //    /// <summary>
    //    /// Content type for serialized content
    //    /// </summary>
    //    public string ContentType { get; set; }
    //}

    //internal class JsonHelper
    //{
    //    private const string INDENT_STRING = "    ";
    //    #region pretty print
    //    public static string FormatJson(string str)
    //    {
    //        var indent = 0;
    //        var quoted = false;
    //        var sb = new StringBuilder();
    //        for (var i = 0; i < str.Length; i++)
    //        {
    //            var ch = str[i];
    //            switch (ch)
    //            {
    //                case '{':
    //                case '[':
    //                    sb.Append(ch);
    //                    if (!quoted)
    //                    {
    //                        sb.AppendLine();
    //                        Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
    //                    }
    //                    break;
    //                case '}':
    //                case ']':
    //                    if (!quoted)
    //                    {
    //                        sb.AppendLine();
    //                        Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
    //                    }
    //                    sb.Append(ch);
    //                    break;
    //                case '"':
    //                    sb.Append(ch);
    //                    bool escaped = false;
    //                    var index = i;
    //                    while (index > 0 && str[--index] == '\\')
    //                        escaped = !escaped;
    //                    if (!escaped)
    //                        quoted = !quoted;
    //                    break;
    //                case ',':
    //                    sb.Append(ch);
    //                    if (!quoted)
    //                    {
    //                        sb.AppendLine();
    //                        Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
    //                    }
    //                    break;
    //                case ':':
    //                    sb.Append(ch);
    //                    if (!quoted)
    //                        sb.Append(" ");
    //                    break;
    //                default:
    //                    sb.Append(ch);
    //                    break;
    //            }
    //        }
    //        return sb.ToString();
    //    }
    //    #endregion
    //}

    //internal static class Extensions
    //{
    //    public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
    //    {
    //        foreach (var i in ie)
    //        {
    //            action(i);
    //        }
    //    }
    //}
    //#endregion

    // Copyright (c) Philipp Wagner. All rights reserved.
    // Licensed under the MIT license. See LICENSE file in the project root for full license information.
    internal class NewtonsoftJsonSerializer : ISerializer, IDeserializer
    {
        private JsonNet serializer;

        internal NewtonsoftJsonSerializer(JsonNet serializer, bool pretty = false)
        {
            ContentType = "application/json";
            this.serializer = serializer;
            if (pretty)
                this.serializer.Formatting = Formatting.Indented; 
        }

        public NewtonsoftJsonSerializer(bool pretty = false) : this(new JsonNet()
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        }, pretty)
        { }

        public string ContentType
        {
            get { return "application/json"; } // Probably used for Serialization?
            set { }
        }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    serializer.Serialize(jsonTextWriter, obj);

                    return stringWriter.ToString();
                }
            }
        }

        public T Deserialize<T>(IRestResponse response)
        {
            var content = response.Content;

            using (var stringReader = new StringReader(content))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        internal object Deserialize(string jsonString, Type t)
        {
            using (var stringReader = new StringReader(jsonString))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return serializer.Deserialize(jsonTextReader, t);
                }
            }
        }
        //public static NewtonsoftJsonSerializer Default
        //{
        //    get
        //    {
        //        return new NewtonsoftJsonSerializer(new Newtonsoft.Json.JsonSerializer()
        //        {
        //            NullValueHandling = NullValueHandling.Ignore,
        //        });
        //    }
        //}
    }

    
}

