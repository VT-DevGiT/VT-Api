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

        public RepeatingBehaviour() : base() { }
        #endregion

        #region Methods
        public override void Kill()
        {
            try
            {
                OnDisable();
                base.Kill();
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Vt-Event: RepeatingBehaviour kill faild!!\n{e}\nStackTrace:\n{e.StackTrace}");
            }
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
            if (_Started)
            {
                CancelInvoke("BehaviourAction");
               _Started = false;
            }
        }
        #endregion
    }
}
