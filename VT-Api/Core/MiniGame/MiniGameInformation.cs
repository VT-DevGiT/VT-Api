using System;

namespace VT_Api.Core.MiniGame
{
    public class MiniGameInformation
    {
        public MiniGameInformation(string name, int id, Type script)
        {
            ID = id;
            Name = name;
            MiniGameScript = script;
        }

        public int ID { get; internal set; }
        public string Name { get; internal set; }
        public Type MiniGameScript { get; internal set; }
    }
}