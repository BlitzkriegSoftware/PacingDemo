using System;

namespace BlitzkriegSoftware.Pacing.Library
{
    public class PacerAgent
    {

        #region "Fields"
        
        private Models.RedisConfiguration _redisconfig;
        private string _keyprefix;
        private BlitzRedisClient _client;
        #endregion

        #region "CTOR"

        private PacerAgent() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="redisConfiguration"></param>
        /// <param name="keyPrefix"></param>
        public PacerAgent(Models.RedisConfiguration redisConfiguration, string keyPrefix)
        {
            _redisconfig = redisConfiguration;
            _client = new BlitzRedisClient(_redisconfig);
            _keyprefix = keyPrefix;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Is a Valid Pacer Agent
        /// </summary>
        public bool IsValid
        {
            get
            {
                return _redisconfig.IsValid && !string.IsNullOrWhiteSpace(this._keyprefix);
            }
        }

        /// <summary>
        /// Make Redis Key
        /// </summary>
        /// <param name="keySuffix"></param>
        /// <returns></returns>
        private string MakeRedisKey(string keySuffix)
        {
            return $"{this._keyprefix}-{keySuffix}";
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Runnable
        /// </summary>
        /// <param name="keySuffix">Suffix</param>
        /// <returns>Fully formed key</returns>
        public bool Runnable(string keySuffix)
        {
            bool isRunnable = false;

            if (string.IsNullOrWhiteSpace(keySuffix)) throw new ArgumentNullException(nameof(keySuffix));

            var key = MakeRedisKey(keySuffix);
            var item = _client.Get<DateTime>(key);

            if (item < DateTime.UtcNow) isRunnable = true;

            return isRunnable;
        }

        /// <summary>
        /// MarkPacing
        /// </summary>
        /// <param name="keySuffix">Suffix</param>
        /// <param name="fromNow">TimeSpan</param>
        public void MarkPacing(string keySuffix, TimeSpan fromNow)
        {
            if (string.IsNullOrWhiteSpace(this._keyprefix)) throw new ArgumentNullException(nameof(this._keyprefix));
            
            if (string.IsNullOrWhiteSpace(keySuffix)) throw new ArgumentNullException(nameof(keySuffix));

            if (fromNow.TotalMilliseconds <= 0.0d) throw new ArgumentNullException(nameof(fromNow));

            var key = MakeRedisKey(keySuffix);
            var dt = DateTime.UtcNow.Add(fromNow);

            _client.Set<DateTime>(key, dt);
        }

        #endregion

    }
}
