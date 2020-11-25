using System;
namespace Edmicro.Core.Caching
{
    /// <summary>
    ///
    /// </summary>
    public class DataCache
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="AbsoluteExpirationRelativeToNow"></param>
        public DataCache(string key, byte[] value, TimeSpan AbsoluteExpirationRelativeToNow)
        {
            _id = Guid.NewGuid().ToString();
            data = value;
            this.key = key;
            timeCreate = DateTime.Now;
            expiration = timeCreate.AddSeconds(AbsoluteExpirationRelativeToNow.TotalSeconds);
            Refresh();
        }
        /// <summary>
        /// Gets or sets the identifier | stepId
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string _id { set; get; }
        /// <summary>
        ///
        /// </summary>
        public byte[] data { set; get; }
        /// <summary>
        /// Thời gian khởi tạo
        /// </summary>
        public DateTime expiration { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string key { set; get; }
        /// <summary>
        ///
        /// </summary>
       
        public DateTime timeCreate { set; get; } = DateTime.Now;
        /// <summary>
        ///
        /// </summary>
        public TimeSpan? AbsoluteExpirationRelativeToNow()
        {
            return expiration - DateTime.Now;
        }
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool isExpiration() => DateTime.Now > expiration;
        /// <summary>
        ///
        /// </summary>
        public void Refresh()
        {
            var expirationSecond = (expiration - timeCreate).TotalSeconds;
            timeCreate = DateTime.Now;
            if (expirationSecond > 0)
            {
                expiration = timeCreate.AddSeconds(expirationSecond);
            }
        }
    }
}