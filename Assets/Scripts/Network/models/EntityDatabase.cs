using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [System.Serializable]
    public class EntityDatabase
    {
        public User[] users;
        public Entity[] roads;
        public Entity[] buildings;
        public Entity[] props;
    }
}
