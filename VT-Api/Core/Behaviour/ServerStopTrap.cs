using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Core.Behaviour
{
    internal class ServerStopTrap : MonoBehaviour
    {
        void OnApplicationQuit() => VtController.Get.Events.Server.InvokeServerStopEvent();
    }
}
