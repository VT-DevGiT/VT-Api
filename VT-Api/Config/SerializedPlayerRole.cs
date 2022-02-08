using Synapse.Api;
using Synapse.Api.Enum;
using Synapse.Api.Items;
using Synapse.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VT_Api.Extension;

namespace VT_Api.Config
{
    [Serializable]
    public class SerializedPlayerRole
    {

        #region Properties & Variable
        public float? Health { get; set; }

        public float? MaxHealth { get; set; }

        public float? ArtificialHealth { get; set; }

        public int? MaxArtificialHealth { get; set; }

        public SerializedVector2 Rotation { get; set; }

        public List<SerializedMapPoint> SpawnPoints { get; set; }

        public SerializedPlayerInventory Inventory { get; set; }
        #endregion

        #region Constructor & Destructor
        public SerializedPlayerRole() { }
        #endregion

        #region Methods
        public void Apply(Player player)
        {
            if (Health != null)
                player.Health = (float)Health;
            if (MaxHealth != null)
                player.MaxHealth = (float)MaxHealth;
            if (MaxHealth != null)
                player.ArtificialHealth = (float)ArtificialHealth;
            if (MaxArtificialHealth != null)
                player.MaxArtificialHealth = (int)MaxArtificialHealth;
            if (Rotation != null)
                player.Rotation = Rotation.Parse();
            if (Inventory != null)
                Inventory.Apply(player);
            if (SpawnPoints != null && SpawnPoints.Any())
                player.Position = SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Count)].Parse().Position;
        }
        #endregion

    }
}
