using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Core.Structur
{
    public class StructureManager
    {
        public GameObject Primitve { get; set; }
        public GameObject Light { get; set; }
        public GameObject TargetSport { get; set; }
        public GameObject TargetHumain { get; set; }

        public List<GameObject> AllSpawnblePrefabs { get => CustomNetworkManager.singleton.spawnPrefabs; }


        public StructureManager()
        {
            Primitve = NetworkManager.singleton.spawnPrefabs.First(x => x.name.Contains("Primitive"));
            Light = NetworkManager.singleton.spawnPrefabs.First(x => x.name.Contains("LightSource"));
            TargetSport = NetworkManager.singleton.spawnPrefabs.First(x => x.name.Contains("sportTarget"));
            TargetHumain = NetworkManager.singleton.spawnPrefabs.First(x => x.name.Contains("dboyTarget"));

        }

        public T SpawnPrefab<T>(string name, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            var obj = UnityEngine.Object.Instantiate(NetworkManager.singleton.spawnPrefabs.First(p => p.name == name));
            obj.gameObject.transform.localScale = scale;
            obj.gameObject.transform.position = position;
            obj.gameObject.transform.rotation = Quaternion.Euler(rotation);
            NetworkServer.Spawn(obj);
            return obj.GetComponent<T>();
        }

    }
}
