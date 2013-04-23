// -----------------------------------------------------------------------
// <copyright file="CacheService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.Core
{
    using System;

    using Fanex.CRM.CRMReplication.Core.Extensions;
    using Fanex.Caching;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CacheService
    {
        private static readonly ICache _cache;

        static CacheService()
        {
            _cache = Fanex.Caching.CacheService.CreateInstance(CacheType.MemCache);
        }

        public static object Get(string key)
        {
            try
            {
                return _cache.Get<object>(key);
            }
            catch (Exception exception)
            {
                LogExtensions.ExceptionHandler(exception);
                return null;
            }

        }

        public static void Set(string key, object value)
        {
            try
            {
                _cache.Set(key, value);
            }
            catch (Exception exception)
            {
                LogExtensions.ExceptionHandler(exception);

            }

        }

        public static T Get<T>(string key)
        {

            return _cache.Get<T>(key);


        }

        public static void Set<T>(string key, T value)
        {
            try
            {


                _cache.Set<T>(key, value);
            }
            catch (Exception exception)
            {
                LogExtensions.ExceptionHandler(exception);
            }
        }

        public static void Set(string key, object value, TimeSpan expire)
        {
            try
            {


                _cache.Set(key, value, expire);
            }
            catch (Exception exception)
            {
                LogExtensions.ExceptionHandler(exception);

            }
        }

        public static void Set<T>(string key, T value, TimeSpan expire)
        {
            _cache.Set<T>(key, value, expire);
        }

        public static void Delete(string key)
        {
            _cache.Clear();
        }

    }
}
