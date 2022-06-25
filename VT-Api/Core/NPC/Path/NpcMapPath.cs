using Synapse.Api.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Core.NPC
{
    public class NpcMapPath
    {
        #region Attributes & Properties
        public string Name { get; set; }
        public List<NpcPathPoint> Points { get; } = new List<NpcPathPoint>();
        public List<KeyValuePair<NpcPathPoint, NpcPathPoint>> Path { get; } = new List<KeyValuePair<NpcPathPoint, NpcPathPoint>>();
        public List<NpcMapPath> LinkedZone { get; } = new List<NpcMapPath>();
        #endregion

        #region Constructors & Destructor
        public NpcMapPath(string name)
        {
            Name = name;
            NpcManger.Get.NpcMaps.Add(this);
        }
        #endregion

        #region Methods
        public void AddPath(NpcPathPoint beginning, NpcPathPoint end, bool reciprocal = true)
        {
            if (beginning.Zone == null)
                beginning.Zone = this;
            if (end.Zone == null)
                end.Zone = this;
            
            Path.Add(new KeyValuePair<NpcPathPoint, NpcPathPoint>(beginning, end));
            if (reciprocal)
                Path.Add(new KeyValuePair<NpcPathPoint, NpcPathPoint>(end, beginning));
        }

        private float Distance(NpcPathPoint beginning, NpcPathPoint end) 
            => Vector3.Distance(beginning.Postion, end.Postion);

        public float Distance(List<NpcPathPoint> path)
        {
            var resultat = 0f;
            for (int i = 0; i < path.Count - 1; i++)
                resultat += Distance(path[i], path[i + 1]);
            return resultat;
        }

        public List<NpcPathPoint> ShorterPath(NpcPathPoint beginning, NpcPathPoint end)
        {
            if (beginning.Zone != end.Zone)
                return null;

            var resultat = new List<List<NpcPathPoint>>();
            GetPaths(beginning, end, new List<NpcPathPoint>(), ref resultat);

            if (!resultat.Any())
                return null;

            var orderBy = resultat.OrderBy(p => Distance(p));
            return orderBy.First();
        }

        private void GetPaths(NpcPathPoint beginning, NpcPathPoint end, List<NpcPathPoint> alreadyPass, ref List<List<NpcPathPoint>> result)
        {
            if (beginning == end)
            {
                alreadyPass.Add(beginning);
                result.Add(alreadyPass);
            }

            var listPossible = Path.Where(p => p.Key == beginning && !alreadyPass.Any(pt => pt == p.Value));
            if (!listPossible.Any())
                return;

            foreach (var element in listPossible)
            {
                List<NpcPathPoint> path = new List<NpcPathPoint>();
                path.AddRange(alreadyPass);
                path.Add(beginning);
                var newPtDepart = Points.First(p => element.Value == p);
                GetPaths(newPtDepart, end, path, ref result);
            }
        }
        #endregion
    }
}
