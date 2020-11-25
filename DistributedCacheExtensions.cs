using System;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace DistributedCache
{
    /// <summary>
    ///
    /// </summary>
    public static class DistributedCacheExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpirationRelativeToNow"></param>
        /// <returns></returns>
        public static void Set<T>(this IDistributedCache cache, string cacheKey, T value, TimeSpan absoluteExpirationRelativeToNow)
        {
            if (cache != null)
            {
                var arrayByte = ToByteArray(value);
                cache.Set(cacheKey, arrayByte, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
                });
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="cacheKey"></param>
        /// <param name="returnOut"></param>
        /// <returns></returns>
        public static bool TryGetValue<T>(this IDistributedCache cache, string cacheKey, out T returnOut)
        {
            returnOut = default(T);
            if (cache != null && !string.IsNullOrWhiteSpace(cacheKey))
            {
                try
                {
                    var arrayByte = cache.Get(cacheKey);
                    if (arrayByte != null)
                    {
                        returnOut = FromByteArray<T>(arrayByte);
                        if (returnOut != null)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
            return false;
        }
        private static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
            {
                return default(T);
            }
            string strJson = System.Text.Encoding.UTF8.GetString(data);
            var _return = JsonConvert.DeserializeObject<T>(strJson);
            return _return;
        }
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
            {
                return null;
            }
            var strJson = JsonConvert.SerializeObject(obj);
            var res = System.Text.Encoding.UTF8.GetBytes(strJson);
            return res;
        }
    }
}