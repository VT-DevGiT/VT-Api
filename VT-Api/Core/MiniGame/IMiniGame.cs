﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.MiniGame
{
    internal interface IMiniGame
    {
        void Start();
        int GetMiniGameID();
        string GetMiniGameName();


        bool RoundEnd { get; set; }
        RoundSummary.LeadingTeam GetLeadingTeam();
    }
}
