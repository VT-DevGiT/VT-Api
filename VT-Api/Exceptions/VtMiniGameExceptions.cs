using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VT_Api.Core.MiniGame;

namespace VT_Api.Exceptions
{
    [Serializable]
    internal class VtMiniGameNotFoundException : Exception
    {

        #region Properties & Variable
        public int ID { get; }
        public string Name { get; }
        #endregion

        #region Constructor & Destructor
        public VtMiniGameNotFoundException(string message, int id) : base(message) => ID = id;
        public VtMiniGameNotFoundException(string message, string name) : base(message) => Name = name;
        public VtMiniGameNotFoundException(string message, int id, string name) : base(message)
        {
            ID = id;
            Name = name;
        }

        protected VtMiniGameNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion

    }
    [Serializable]
    internal class VtMiniGameAlreadyRegisteredException : Exception
    {
        #region Properties & Variable
        public MiniGameInformation MiniGameInformation { get; }
        #endregion

        #region Constructor & Destructor
        public VtMiniGameAlreadyRegisteredException(string message, MiniGameInformation info) : base(message) => MiniGameInformation = info;
        protected VtMiniGameAlreadyRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion
    }
}
