using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.MiniGame
{
    public abstract class AbstractMinGame : IMiniGame
    {
        public AbstractMinGame() { }

        public bool RoundEnd { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public RoundSummary.LeadingTeam GetLeadingTeam()
        {
            throw new NotImplementedException();
        }

        public int GetMiniGameID()
        {
            throw new NotImplementedException();
        }

        public string GetMiniGameName()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}
