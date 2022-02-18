using Synapse.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Core.Behaviour
{
    public class Display : MonoBehaviour
    {
        Player player;

        private void Start()
        {
            player = this.gameObject.GetPlayer();

            if (player == null)
                throw new Exception("Behaviour \"Display\" is not on a player !");
        }
 

    }
}
