using System;
using System.Reflection;

namespace VT_Api.Extension
{
    static internal class VtExtensions
    {
        internal static void Debug(this Synapse.Api.Logger logger, object message)
            => logger.Send($"VtApi-Debug: {message}", ConsoleColor.DarkYellow);




    }
}