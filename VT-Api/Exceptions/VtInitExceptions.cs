using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Exceptions
{
    [Serializable]
    public class VtInitAllHandlerExceptions : Exception
    {
        public VtInitAllHandlerExceptions() { }
        public VtInitAllHandlerExceptions(string message) : base(message) { }
        public VtInitAllHandlerExceptions(string message, Exception inner) : base(message, inner) { }
        protected VtInitAllHandlerExceptions(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class VtInitPatchsException : Exception
    {
        public VtInitPatchsException() { }
        public VtInitPatchsException(string message) : base(message) { }
        public VtInitPatchsException(string message, Exception inner) : base(message, inner) { }
        protected VtInitPatchsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
