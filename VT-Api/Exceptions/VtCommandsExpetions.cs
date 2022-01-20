using Synapse.Command;
using System;

namespace VT_Api.Exceptions
{
    public class VtMainCommandsDontExistExpetions : Exception
    {
        public VtMainCommandsDontExistExpetions(string message, string name) : base(message) => Name = name;
        public string Name { get; }
    }

    internal class VtCommandAlreadyRegisteredException : Exception
    {
        public VtCommandAlreadyRegisteredException(string message, CommandInformation info) : base(message) => CommandInformation = info;

        public CommandInformation CommandInformation { get; }
    }
}
