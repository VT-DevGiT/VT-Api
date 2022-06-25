using Synapse;
using Synapse.Api;
using Synapse.Api.Events.SynapseEventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VT_Api.Core.Behaviour;
using VT_Api.Core.Enum;
using VT_Api.Core.Events.EventArguments;
using VT_Api.Extension;

namespace VT_Api.Core.Roles
{
    //TODO:
    /*
     * Refresh when :
     * Player change name
     * Player change role
     * Player change rank
     * Player change rank color
     * Player change custome info
     * Player change heriarchie
     * Player change custom info
     * hide area
     * 
     * Patch for 
     * refresh
     * RoleName
     * CustomInfo
     * HierarchyPower
     * Player.HideRank for hide area
     * hide area
     * 
     * 
     * 
     */
    public class CustomDisplay : RepeatingBehaviour
    {
        #region Properties & Variable
        public Player Player { get; private set; }

        bool roleNameInfoIsSet;
        string roleName;
        public string RoleName 
        { 
            get
            {
                return roleName;
            }
            set
            {
                roleNameInfoIsSet = !string.IsNullOrEmpty(value);
                roleName = value;
            }
        }

        bool custiomInfoIsSet;
        string customInfo;
        public string CustomInfo
        {
            get
            {
                return customInfo;
            }
            set
            {
                custiomInfoIsSet = !string.IsNullOrEmpty(value);
                customInfo = value;
            }
        }

        bool hierarchyPowerIsSet;
        int hierarchyPower;
        public int HierarchyPower
        {
            get
            {
                return hierarchyPower;
            }
            set
            {
                hierarchyPowerIsSet = value != -1;
                hierarchyPower = value;
            }
        }

        const int delata = 5; //this number must be a divisor of 255 (for the rambo color)
        private float delay = Time.time;
        int r = 255, g = 0, b = 0;
        #endregion

        #region Constructor & Destructor
        public CustomDisplay()
        {
            this.RefreshTime = delata * 1000;
        }
        #endregion

        #region Methods
        private void Start()
        {
            Player = this.gameObject.GetPlayer();

            if (Player == null)
            {
                enabled = false;
                throw new Exception("Behaviour \"Display\" is not on a player !");
            }
        }

        protected override void BehaviourAction()
        {
            if (Time.time >= delay)
            {
                delay += delata;
                if (r > 0 && b == 0)
                {
                    r -= delata;
                    g += delata;
                }

                if (g > 0 && r == 0)
                {
                    g -= delata;
                    b += delata;
                }

                if (b > 0 && g == 0)
                {
                    b -= delata;
                    r += delata;
                }

                Refresh(Server.Get.Players.Where(p => p.RoleType != RoleType.Spectator).ToList());
            }
        }

        public string BuildNickName()
        {
            if ((Player.NicknameSync.Network_playerInfoToShow & PlayerInfoArea.Nickname) == PlayerInfoArea.Nickname)
                return Player.name + "\n";
            return String.Empty;
        }

        public string BuildBadge()
        {
            if ((Player.NicknameSync.Network_playerInfoToShow & PlayerInfoArea.Badge) == PlayerInfoArea.Badge && !string.IsNullOrEmpty(Player.RankName))
            {
                //TODO: RAMBO COLOR
                if (Player.RankColor == "RAINBOW")
                {
                    var color = $"#{r:X2}{g:X2}{b:X2}";
                    return $"<color={color}>{Player.RankName}</color>\n";
                }
                else
                {
                    return $"<color={Player.RankColor}>{Player.RankName}</color>\n";
                }
            }
            return string.Empty;
        }

        public string BuildRole()
        {
            if (roleNameInfoIsSet && (Player.NicknameSync.Network_playerInfoToShow & PlayerInfoArea.Role) == PlayerInfoArea.Role)
            {
                return roleName;
            }
            return string.Empty;
        }

        public string BuildUnite()
        {
            if ((Player.NicknameSync.Network_playerInfoToShow & PlayerInfoArea.UnitName) == PlayerInfoArea.UnitName && !string.IsNullOrEmpty(Player.UnitName))
               return $" ({Player.UnitName})";
            return string.Empty;
        }

        public string BuildCustomInfo()
        {
            if (custiomInfoIsSet && (Player.NicknameSync.Network_playerInfoToShow & PlayerInfoArea.CustomInfo) == PlayerInfoArea.CustomInfo)
                return $"\n{customInfo}";
            return string.Empty;
        }

        public void Refresh(List<Player> players) // Yes is a copy past of Refresh but maby for futur update add diffenrent builder for each player
        {
            if (Player.RankColor == "RAINBOW")
                enabled = true;

            string display = string.Empty;
            display += BuildNickName();
            display += BuildBadge();
            display += BuildRole();
            display += BuildUnite();
            display += BuildCustomInfo();

            //power statue
            if (hierarchyPowerIsSet && (Player.NicknameSync.Network_playerInfoToShow & PlayerInfoArea.PowerStatus) == PlayerInfoArea.PowerStatus)
            { 
                foreach (var player in players)
                {
                    string displayWhitHierachy = display;
                    var hierarchy = RoleManager.Get.GetHierachy(player.RoleID);
                    if (hierarchy != -1)
                    {
                        display += "\n";
                        if (hierarchy > HierarchyPower)
                            displayWhitHierachy += Config.Config.Get.VtTranslation.ActiveTranslation.RankUnder;
                        else if (hierarchy < HierarchyPower)
                            displayWhitHierachy += Config.Config.Get.VtTranslation.ActiveTranslation.RankOver;
                        else
                            displayWhitHierachy += Config.Config.Get.VtTranslation.ActiveTranslation.RankSame;
                    }
                    NetworkLiar.Get.SendDisplayInfo(Player, displayWhitHierachy, new List<Player>() { player });
                    NetworkLiar.Get.SendInfoToDisplay(Player, PlayerInfoArea.CustomInfo, Server.Get.Players);
                }
            }
            else
            {
                NetworkLiar.Get.SendDisplayInfo(Player, display, Server.Get.Players);
                NetworkLiar.Get.SendInfoToDisplay(Player, PlayerInfoArea.CustomInfo, Server.Get.Players);
            }
        }
        #endregion
    }
}
