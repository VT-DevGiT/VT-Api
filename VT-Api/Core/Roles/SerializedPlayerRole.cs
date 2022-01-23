using Synapse.Api;
using Synapse.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Core.Roles
{
    public class SerializedPlayerRole
    {
        public SerializedPlayerRole() { }
             
        public SerializedPlayerRole(SerializedMapPoint spawnPoint) 
            => SpawnPoint = spawnPoint;

        public SerializedPlayerRole(SerializedMapPoint spawnPoint, SerializedVector2 rotation) 
            : this(spawnPoint)
           => Rotation = rotation;

        public SerializedPlayerRole(SerializedMapPoint spawnPoint, SerializedVector2 rotation, int health) 
            : this (spawnPoint, rotation) 
            => Health = health;

        public SerializedPlayerRole(SerializedMapPoint spawnPoint, SerializedVector2 rotation, int health, int maxHealth) 
            : this(spawnPoint, rotation, health) 
            => MaxHealth = maxHealth;

        public SerializedPlayerRole(SerializedMapPoint spawnPoint, SerializedVector2 rotation, int health, int maxHealth, int artificialHealth) 
            : this(spawnPoint, rotation, health, maxHealth) 
            => ArtificialHealth = artificialHealth;

        public SerializedPlayerRole(SerializedMapPoint spawnPoint, SerializedVector2 rotation, int health, int maxHealth, int artificialHealth, int maxArtificialHealth) 
            : this(spawnPoint, rotation, health, maxHealth, artificialHealth) 
            => MaxArtificialHealth = maxArtificialHealth;

        public float? Health { get; set; }

        public float? MaxHealth { get; set; }

        public float? ArtificialHealth { get; set; }

        public int? MaxArtificialHealth { get; set; }

        public SerializedVector2 Rotation { get; set; }

        public SerializedMapPoint SpawnPoint { get; set; }

        public SerializedPlayerInventory Inventory { get; set; }

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

            player.Position = SpawnPoint.Parse().Position;
        }
    }
}
