using Synapse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.NPC
{
    public class NpcManger
    {
        #region Properties & Variable
        public Dictionary<uint, NPC> NPCs { get; } = new Dictionary<uint, NPC>();
        public List<NpcPathPoint> NpcPathPoints { get; } = new List<NpcPathPoint>();
        public List<NpcMapPath> NpcMaps { get; } = new List<NpcMapPath>();

        public static NpcManger Get => VtController.Get.Npc;
        #endregion

        #region Constructor & Destructor
        internal NpcManger() { }
        #endregion

        #region Methods
        internal void Init()
        {
            Server.Get.Events.Round.RoundEndEvent += OnEnd;
            Server.Get.Events.Round.WaitingForPlayersEvent += OnWhait;
        }

        private void OnWhait()
        {

        }

        private void OnEnd()
        {
            NPC.ResetID();
            NpcPathPoint.ResetID();
            foreach (var npc in NPCs)
                npc.Value.Despawn();
            NPCs.Clear();
            NpcPathPoints.Clear();
            NpcMaps.Clear();
        }
        #endregion

        #region Events
        #endregion



    }
}
