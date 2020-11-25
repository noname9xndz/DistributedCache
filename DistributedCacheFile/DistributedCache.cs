using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Edmicro.Core.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace DistributedCache.DistributedCacheFile
{
    /// <summary>
    ///
    /// </summary>
    public class DistributedCache : IDistributedCache
    {
        private readonly DistributedCacheFileConfig _config;
        private string GetPathFileByKey(string key)
        {
            string pathFile = Path.Combine(_config.DirectoryCache, key);
            return pathFile;
        }
        /// <summary>
        ///
        /// </summary>
        public DistributedCache(IOptions<DistributedCacheFileConfig> options)
        {
            _config = new DistributedCacheFileConfig
            {
                DirectoryCache = "cache"
            };
            if (options != null && options.Value != null)
            {
                _config = options.Value;
            }
            DistributedCacheHeplers.CreateDirectoryIfMissing(_config.DirectoryCache);
        }
        private void SetFolder(string cacheKey)
        {
            var listStr = cacheKey.Replace("//", "/").Replace("/", "\\").Split("\\").ToList();
            string folderPath = string.Join("\\", listStr.SkipLast(1));
            folderPath = Path.Combine(_config.DirectoryCache, folderPath);
            DistributedCacheHeplers.CreateDirectoryIfMissing(folderPath);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] Get(string key)
        {
            var objCache = GetDataCache(key);
            if (objCache != null)
            {
                return objCache.data;
            }
            return null;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            var objCache = await GetDataCacheAsync(key);
            if (objCache != null)
            {
                return objCache.data;
            }
            return null;
        }
        private DataCache GetDataCache(string key)
        {
            var pathFile = GetPathFileByKey(key);
            if (File.Exists(pathFile))
            {
                var strJson = ReadJson(pathFile);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    var objCache = DistributedCacheHeplers.DeserializeObjectCheckFormat<DataCache>(strJson);
                    if (objCache != null && objCache.isExpiration())
                    {
                        Remove(key);
                        return null;
                    }
                    return objCache;
                }
                else
                {
                    Remove(key);
                }
            }
            return null;
        }
        private string ReadJson(string pathFile)
        {
            string fileContents;
            var fileInfo = new FileInfo(pathFile);
            using (var fs = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    fileContents = reader.ReadToEnd();
                }
            }
            return fileContents;
        }
        private async Task<DataCache> GetDataCacheAsync(string key)
        {
            var pathFile = GetPathFileByKey(key);
            if (File.Exists(pathFile))
            {
                var strJson = await ReadJsonAsync(pathFile);
                if (!string.IsNullOrWhiteSpace(strJson))
                {
                    var objCache = DistributedCacheHeplers.DeserializeObjectCheckFormat<DataCache>(strJson);
                    if (objCache != null && objCache.isExpiration())
                    {
                        await RemoveAsync(key);
                        return null;
                    }
                    return objCache;
                }
                else
                {
                    await RemoveAsync(key);
                }
            }
            return null;
        }
        private async Task<string> ReadJsonAsync(string pathFile)
        {
            string fileContents;
            var fileInfo = new FileInfo(pathFile);
            using (var fs = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    fileContents = await reader.ReadToEndAsync();
                }
            }
            return fileContents;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        public void Refresh(string key)
        {
            var objCache = GetDataCache(key);
            if (objCache != null)
            {
                var pathFile = GetPathFileByKey(key);
                objCache.Refresh();
                WriteFile(pathFile, objCache);
            }
        }
        private void WriteFile(string pathFile, DataCache data)
        {
            var strJson = System.Text.Json.JsonSerializer.Serialize(data);
            File.WriteAllText(pathFile, strJson, System.Text.Encoding.UTF8);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            var objCache = await GetDataCacheAsync(key);
            if (objCache != null)
            {
                var pathFile = GetPathFileByKey(key);
                objCache.Refresh();
                await WriteFileAsync(pathFile, objCache);
            }
        }
        private Task WriteFileAsync(string pathFile, DataCache data)
        {
            var strJson = System.Text.Json.JsonSerializer.Serialize(data);
            return File.WriteAllTextAsync(pathFile, strJson, System.Text.Encoding.UTF8);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            var pathFile = GetPathFileByKey(key);
            File.Delete(pathFile);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            Remove(key);
            return Task.FromResult(1);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public void Set(string key, byte[] value, DistributedCacheEntryOptions options) => Set(key, value, options.AbsoluteExpirationRelativeToNow);
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="AbsoluteExpirationRelativeToNow"></param>
        public void Set(string key, byte[] value, TimeSpan? AbsoluteExpirationRelativeToNow)
        {
            if (!string.IsNullOrWhiteSpace(key) && value != null)
            {
                SetFolder(key);
                var pathFile = GetPathFileByKey(key);
                if (AbsoluteExpirationRelativeToNow == null)
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                }
                var data = new DataCache(key, value, AbsoluteExpirationRelativeToNow.Value);
                WriteFile(pathFile, data);
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default) => SetAsync(key, value, options.AbsoluteExpirationRelativeToNow);
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="AbsoluteExpirationRelativeToNow"></param>
        /// <returns></returns>
        public async Task SetAsync(string key, byte[] value, TimeSpan? AbsoluteExpirationRelativeToNow)
        {
            if (!string.IsNullOrWhiteSpace(key) && value != null)
            {
                var pathFile = GetPathFileByKey(key);
                if (AbsoluteExpirationRelativeToNow == null)
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                }
                var data = new DataCache(key, value, AbsoluteExpirationRelativeToNow.Value);
                await WriteFileAsync(pathFile, data);
            }
        }
    }
}