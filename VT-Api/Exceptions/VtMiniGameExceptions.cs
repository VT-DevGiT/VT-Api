using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VT_Api.Core.MiniGame;

namespace VT_Api.Exceptions
{
    internal class VtMiniGameNotFoundException : Exception
    {
        public VtMiniGameNotFoundException(string message, int id) : base(message) => ID = id;

        public VtMiniGameNotFoundException(string message, string name) : base(message) => Name = name;

        public VtMiniGameNotFoundException(string message, int id,  string name) : base(message)
        {
            ID = id;
            Name = name;
        }

        public int ID { get; }

        public string Name { get; set; }
    }

    internal class VtMiniGameAlreadyRegisteredException : Exception
    {
        public VtMiniGameAlreadyRegisteredException(string message, MiniGameInformation info) : base(message) => MiniGameInformation = info;

        public MiniGameInformation MiniGameInformation { get; }
    }
}
