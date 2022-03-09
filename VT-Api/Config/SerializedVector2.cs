using Synapse.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VT_Api.Config
{
    [Serializable]
    public class SerializedVector2
    {
        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        public SerializedVector2(Vector2 vector)
        {
            X = vector.x;
            Y = vector.y;
        }

        public SerializedVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public SerializedVector2() { }

        public Vector2 Parse() => new Vector2(X, Y);
        

        public static implicit operator Vector2(SerializedVector2 vector) => vector.Parse();
        public static implicit operator SerializedVector2(Vector2 vector) =>  new SerializedVector2(vector);
        
    }
}
