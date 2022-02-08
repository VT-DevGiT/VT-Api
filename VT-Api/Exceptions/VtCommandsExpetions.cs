using Synapse.Command;
using System;
using System.Runtime.Serialization;

namespace VT_Api.Exceptions
{
    [Serializable]
    public class VtMainCommandsDontExistExpetions : Exception
    {

        #region Properties & Variable
        public string Name { get; }
        #endregion

        #region Constructor & Destructor

        public VtMainCommandsDontExistExpetions(string message, string name) : base(message) => Name = name;

        protected VtMainCommandsDontExistExpetions(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion
    }

    [Serializable]
    internal class VtCommandAlreadyRegisteredException : Exception
    {
        #region Properties & Variable
        public CommandInformation CommandInformation { get; }
        #endregion

        #region Constructor & Destructor
        public VtCommandAlreadyRegisteredException(string message, CommandInformation info) : base(message) => CommandInformation = info;
        
        protected VtCommandAlreadyRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion

    }
}
