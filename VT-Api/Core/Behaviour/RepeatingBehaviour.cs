using Mirror;
using Synapse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT_Api.Core.Behaviour
{
    public abstract class RepeatingBehaviour : RoundBehaviour
    {

        /// <summary>
        /// Time in millisecond between behaviour execution
        /// </summary>
        public virtual int RefreshTime { get; set; }

        protected bool _Started;

        #region Constructor & Destructor
        public RepeatingBehaviour(bool killAtRoundStart) : base(killAtRoundStart) { }

        public RepeatingBehaviour() : base(true) { }
        #endregion

        #region Methods
        public override void Kill()
        {
            OnDisable();
            base.Kill();
        }

        /// <summary>
        /// BehaviourAction is the action that will be repeated every RefreshTime milisecond
        /// </summary>
        protected abstract void BehaviourAction();

        protected virtual void OnEnable()
        {
            ActionExecute();
        }

        protected virtual void OnDisable()
        {
            ActionStop();
        }

        protected virtual void Start()
        {
            _Started = false;
            ActionExecute();
        }

        private void ActionExecute()
        {
            if (!_Started)
            {
                _Started = true;
                float delay = ((float)RefreshTime) / 1000;
                InvokeRepeating("BehaviourAction", delay, delay);
            }
        }

        private void ActionStop()
        {
            _Started = false;
            CancelInvoke("BehaviourAction");
        }
        #endregion
    }
}
