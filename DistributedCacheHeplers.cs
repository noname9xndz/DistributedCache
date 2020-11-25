using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DistributedCache
{
    /// <summary>
    /// 
    /// </summary>
    public static class DistributedCacheHeplers
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectoryIfMissing(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            if (path.Contains("."))
            {
                // path = Path File
                string folder = Path.GetDirectoryName(path);
                directoryInfo = new DirectoryInfo(folder ?? string.Empty);
            }
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueJson"></param>
        /// <returns></returns>
        public static T DeserializeObjectCheckFormat<T>(string valueJson)
        {
            if (!string.IsNullOrWhiteSpace(valueJson))
            {
                valueJson = valueJson.Trim();
                if (valueJson.StartsWith("{") && valueJson.EndsWith("}"))
                {
                    //One object
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(valueJson);
                }
                else if (valueJson.StartsWith("[") && valueJson.EndsWith("]"))
                {
                    //List object
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(valueJson);
                }
            }
            return default(T);
        }
    }
}
