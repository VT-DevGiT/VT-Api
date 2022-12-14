using Synapse.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VT_Api.Core.Enum;

namespace VT_Api.Core.NPC
{
    public class NpcPathPoint
    {
        #region Properties & Variable
        private static uint HighestID;
        
        public NpcPathPointType Type { get; private set; }
        public uint ID { get; private set; }
        public Vector3 Postion { get; private set; }
        public Room Room { get; private set; }
        public NpcMapPath Zone { get; internal set; }
        #endregion

        #region Constructor & Destructor
        public NpcPathPoint(NpcPathPointType type, MapPoint mapPoint) : this(type, mapPoint.Position, mapPoint.Room) { }

        public NpcPathPoint(NpcPathPointType type, Vector3 postion, Room room = null)
        {
            Type = type;
            Postion = postion;
            Room = room;
            ID = HighestID;
            HighestID++;
            NpcManger.Get.NpcPathPoints.Add(this);
        }
        #endregion

        #region Methods
        internal static NpcPathPoint Found(Vector3 postion, NpcMapPath zone)
            => zone.Points.OrderBy(p => Vector3.Distance(p.Postion, postion)).FirstOrDefault();
        
        internal static void ResetID() => HighestID = 0;

        #endregion
    }
}
