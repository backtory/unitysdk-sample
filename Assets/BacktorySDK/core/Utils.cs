using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Assets.BacktorySDK.core 
{
    public static class Utils
    {
        public static T checkNotNull<T>(T Object, string message)
        {
            if (Object == null)
            {
                throw new NullReferenceException(message);
            }
            return Object;
        }

        /// <summary>
        /// Returns all the fields annotated with the passed attribute. If obj is null, search on static fileds only
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="obj"></param>
        /// <param name="considerInheritedFields"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetFieldsByAttibute(Type attributeType, object obj, bool considerInheritedFields)
        {
            BindingFlags baseFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            if (obj != null)
                baseFlags |= BindingFlags.Instance;
            var allFields = obj.GetType().GetFields(baseFlags);
            return allFields.Where(fieldInfo => fieldInfo.IsDefined(attributeType, considerInheritedFields));
        }
        
        /// <summary>
        /// Returns all the properties annotated with the passed attribute. If obj is null, search on static properties only
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="obj"></param>
        /// <param name="considerInheritedFields"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertiesByAttribute(Type attributeType, object obj, bool considerInheritedFields)
        {
            BindingFlags baseFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            if (obj != null)
                baseFlags |= BindingFlags.Instance;
            var allProperties = obj.GetType().GetProperties(baseFlags);
            return allProperties.Where(fieldInfo => fieldInfo.IsDefined(attributeType, considerInheritedFields));
        }

        /// <summary>
        /// Returns the single field annotated with the passed attribute. If obj is null, search on static fields only
        /// If no such field returns null. If more than one throws exception.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="obj"></param>
        /// <param name="considerInheritedFields"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldByAttribute(Type attributeType, object obj, bool considerInheritedFields)
        {
            return GetFieldsByAttibute(attributeType, obj, considerInheritedFields).SingleOrDefault();
        }

        /// <summary>
        /// Returns the single property annotated with the passed attribute. If obj is null, search on static property only
        /// If no such property, returns null. If more than one throws exception.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="obj"></param>
        /// <param name="considerInheritedFields"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyByAttribute(Type attributeType, object obj, bool considerInheritedFields)
        {
            return GetPropertiesByAttribute(attributeType, obj, considerInheritedFields).SingleOrDefault();
        }

        ///// <summary>
        ///// Field priority is higher (than property)
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public static T GetFieldOrPropertyValueByAttribute<T>()
    }

    public static class EmptyStringExtension
    {
        public static bool IsEmpty(this string s)
        {
            return s == null || s.All(char.IsWhiteSpace); //string.IsNullOrEmpty(s.Trim() ?? null);
        }
    }
}
