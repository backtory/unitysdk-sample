using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.BacktorySDK.core{

    public interface IStorage
    {
        /// <summary>
        /// Stores a key-value in string format.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void Put(string key, string data);

        /// <summary>
        /// Gets the values associated to <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string Get(string key);

        /// <summary>
        /// Remove the value associated to <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// Clears everything in storage. All user info will be lost after this and user must login again.
        /// </summary>
        void Clear();
    }

    // TODO: not sure Save usage is right :|
    /// <summary>
    /// Implements <see cref="IStorage"/> methods by using <see cref="PlayerPrefs"/> API
    /// </summary>
    public class PlayerPrefsStorage : IStorage
    {
        public void Clear()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        public string Get(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public void Put(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        public void Remove(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }

    public class FileStorage : IStorage
    {
        private static readonly string address = Path.Combine(Application.persistentDataPath, "backtory.storage");
        private Dictionary<string, string> dic;
        
        private void Load()
        {
            if (!File.Exists(address))
            {
                dic = new Dictionary<string, string>();
                return;
            }
            using (var fs = new FileStream(address, FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(fs);
                var jsonString = reader.ReadToEnd();
                reader.Close();
                fs.Close();
                dic = Backtory.FromJson<Dictionary<string, string>>(jsonString) ?? new Dictionary<string, string>();
            }
        }

        private void Save()
        {
            using (var fs = new FileStream(address, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(Backtory.ToJson(dic));
                    writer.Flush();
                    writer.Close();
                    fs.Close();
                }
            }
        }

        public void Clear()
        {
            using (var fs = new FileStream(address, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write("");
                    writer.Flush();
                    writer.Close();
                    fs.Close();
                }
            }
        }

        public string Get(string key)
        {
            Load();
            string value;
            return dic.TryGetValue(key, out value) ? value : null; // preventing key not found exception
        }

        public void Put(string key, string data)
        {
            Load();
            dic[key] = data;
            Save();            
        }

        public void Remove(string key)
        {
            Load();
            if (dic.ContainsKey(key)) {
                dic.Remove(key);
                Save();
            }
        }
    }
}