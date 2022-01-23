using Synapse.Api;
using Synapse.Api.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VT_Api.Core.Enum;
using VT_Api.Core.Roles;
using VT_Api.Core.Teams;

namespace Exemple_Plugin
{
    public class SpyPlayerScript : AbstractRole
    {
        protected override string SpawnMessage => PluginClass.Instance.Config.SpawnMessage;

        protected override int[] EnemysList => TeamManager.Groupe.CHIenemy;

        protected override int[] FriendsList => TeamManager.Groupe.CHIally;

        protected override RoleType RoleType => RoleType.NtfPrivate;

        protected override int RoleTeam => (int)TeamID.CHI;

        protected override int RoleId => (int)RoleID.ChaosSpy; 

        protected override string RoleName => PluginClass.Instance.Translation.ActiveTranslation.ClassName;

        protected override SerializedPlayerRole Config => PluginClass.Instance.Config.SpyRoleConfig;

        bool reveal = false;
        public override bool CallPower(byte power, out string message)
        {
            if (power == 1)
            {
                if (reveal)
                {
                    message = "You already took off your disguise";
                    return false;
                }

                Map.Get.Explode(Player.Position, GrenadeType.Flashbang, Player);
                
                Player.RoleType = RoleType.ChaosConscript;

                message = "You took off your disguise !";
                return true;
            }
            message = "You only ave one power";
            return false;
        }
    }
}
