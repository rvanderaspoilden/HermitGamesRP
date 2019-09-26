using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [System.Serializable]
    public class Vector3Json
    {
        public string x;
        public string y;
        public string z;

        public Vector3Json(string x, string y, string z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3Json(Vector3 data) {
            this.x = data.x.ToString();
            this.y = data.y.ToString();
            this.z = data.z.ToString();
        }
    }
}
