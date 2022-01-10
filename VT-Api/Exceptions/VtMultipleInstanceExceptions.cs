using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Exceptions
{
    public class VtMultipleInstanceExceptions : Exception
    {
        public VtMultipleInstanceExceptions() : base() { }
        internal VtMultipleInstanceExceptions(string message) : base(message) { }
    }
}
