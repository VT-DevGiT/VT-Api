using System;
using System.Runtime.Serialization;

namespace VT_Api.Exceptions
{
    [Serializable]
    internal class VtUnknownVersionException : Exception
    {
        #region Properties & Variable
        public string AssemblyName { get; }
        public string PluginName { get; }
        #endregion

        #region Constructor & Destructor
        public VtUnknownVersionException(string message, string assemblyName) : base(message) => AssemblyName = assemblyName;
        public VtUnknownVersionException(string message, string dllName, string pluginName)
        {
            PluginName = pluginName;
            AssemblyName = dllName;
        }

        protected VtUnknownVersionException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion
    }
}
