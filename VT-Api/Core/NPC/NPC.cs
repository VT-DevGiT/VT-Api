using Synapse.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Core.NPC
{
    public class NPC : Dummy
    {

        #region Properties & Variable

        private static uint HighestID;
        public uint ID { get; private set; }
        public NpcMovementController MovementController { get; private set; }
        public Vector3? GoTo { get => MovementController.GoTo; set => MovementController.GoTo = value; }

        #endregion

        #region Constructor & Destructor
        public NPC(Vector3 pos, Quaternion rot, RoleType role = RoleType.ClassD, string name = "(null)", string badgetext = "", string badgecolor = "") : this(pos, new Vector2(rot.eulerAngles.x, rot.eulerAngles.y), role, name, badgetext, badgecolor) { }

        public NPC(Vector3 pos, Vector2 rot, RoleType role = RoleType.ClassD,  string name = "(null)", string badgetext = "", string badgecolor = "") : base(pos, rot, role, name, badgetext, badgecolor)
        {
            MovementController = GameObject.AddComponent<NpcMovementController>();
            ID = HighestID;
            HighestID++;
        }
        #endregion

        #region Methods
        internal static void ResetID() => HighestID = 0;

        public new static NPC CreateDummy(Vector3 pos, Quaternion rot, RoleType role = RoleType.ClassD, string name = "(null)", string badgetext = "", string badgecolor = "")
            => new NPC(pos, rot, role, name, badgetext, badgecolor);
        #endregion

        #region Events
        #endregion





    }
}
