using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Core.NPC
{
    public class NpcPath
    {
        #region Properties & Variable
        public bool LastTravels { get; private set; }
        public NpcPathPoint Beginning { get; private set; }
        public NpcPathPoint End { get; private set; }
        public Vector3 EndPostion { get; private set; }
        public Vector3 NextPostion { get; private set; }

        public NpcPathPoint _Curent;
        public NpcPathPoint Curent 
        { 
            get => _Curent;
            set
            {
                _Curent = value;
                Refresh();
            }
        }

        public List<NpcPathPoint> Path { get; private set; }
        #endregion

        #region Constructor & Destructor
        public NpcPath(Vector3 beginning, Vector3 end, NpcMapPath map)
        {
            End = NpcPathPoint.Found(end, map);
            Beginning = NpcPathPoint.Found(beginning, map);
            EndPostion = end;
            Curent = null;
            NextPostion = beginning;
            Path = map.ShorterPath(Beginning, End);
        }
        #endregion

        #region Methods
        public static NpcPath FoundPath(Vector3 beginning, Vector3 end, NpcMapPath map)
            => new NpcPath(end, beginning, map);

        public void NextPoint()
        {
            var nextIndex = Path.IndexOf(Curent) + 1;
            if (Path.Count == nextIndex)
            {
                Curent = Path.Last();
                NextPostion = EndPostion;
                LastTravels = true;
                return;
            }
            Curent = Path[nextIndex];
            NextPostion = Path[nextIndex + 1].Postion;
        }

        private void Refresh()
        {
            var index = Path.IndexOf(Curent);
            if (Path.Count == index + 1)
            {
                Curent = Path[index];
                NextPostion = EndPostion;
                LastTravels = true;
                return;
            }
            Curent = Path[index];
            NextPostion = Path[index + 1].Postion;
        }
        #endregion
    }
}
