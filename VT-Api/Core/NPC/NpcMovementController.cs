using Synapse.Api;
using Synapse.Api.Enum;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VT_Api.Core.Behaviour;

namespace VT_Api.Core.NPC
{
    public class NpcMovementController : RepeatingBehaviour
    {
        #region Attributes & Properties

        public NPC NPC { get; private set; }
        
        public NpcMapPath Zone { get; set; }
        public Vector3? GoTo 
        { 
            get => CurentPath?.End.Postion;
            set
            {
                if (value == null)
                { 
                    enabled = false;
                    CurentPath = null;
                }
                else
                {
                    CurentPath = NpcPath.FoundPath(NPC.Position, value.Value, Zone);
                    enabled = true;
                }
            } 
        }
        public NpcPath CurentPath { get; private set; } 


        #endregion

        #region Constructors & Destructor
        NpcMovementController()
        {
            this.RefreshTime = 100;
            var player = gameObject.GetPlayer();
            NPC = (NPC)Map.Get.Dummies.FirstOrDefault(x => x.Player == player);
        }
        #endregion

        #region Methods
        protected override void OnDisable()
        {
            NPC.Direction = MovementDirection.Stop;
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            if (GoTo != null)
                CurentPath = NpcPath.FoundPath(NPC.Position, GoTo.Value, Zone);
            base.OnEnable();
        }

        protected override void BehaviourAction()
        {
            if (CurentPath == null)
                enabled = false;
            NPC.RotateToPosition(CurentPath.NextPostion);
            if (Vector3.Distance(NPC.Position, CurentPath.NextPostion) < 1f)
            {
                if (CurentPath.LastTravels)
                    CurentPath = null;
                else
                    CurentPath.NextPoint(); 
            }
        }
        #endregion
    }
}