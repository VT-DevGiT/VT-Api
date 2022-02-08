using System;
using System.Reflection;

namespace VT_Api.Core
{
    internal class Singleton<T>
        where T : class
    {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = (T)typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null).Invoke(null);
                    return _instance;
                }
            }
        }
    }
}
