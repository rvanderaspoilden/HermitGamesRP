using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [System.Serializable]
    public class Entity
    {
        public string id;
        public string ownerId; // userId or SERVER
        public EntityType type;
        public string prefabName;
        public Vector3Json position;
        public Vector3Json rotation;

        [SerializeField]
        public Dictionary<string, string> state;
    }

    public enum EntityType
    {
        USER,
        ROAD,
        BUILDING,
        PROPS
    }
}
