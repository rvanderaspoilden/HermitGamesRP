using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    public class NetworkIdentity : MonoBehaviour
    {
        [SerializeField] private EntityType type;

        private string networkId; // equals to entityId server side
        private bool isLocal;
        [SerializeField] private bool isMasterClient;

        public void Setup(string id, bool isMine, bool isMasterClient) {
            this.networkId = id;
            this.isLocal = isMine;
            this.isMasterClient = isMasterClient;
        }

        public void SetNetworkID(string id) {
            this.networkId = id;
        }

        public string GetNetworkID() {
            return this.networkId;
        }

        public bool IsMine() {
            return this.isLocal;
        }

        public bool GetIsMasterClient() {
            return this.isMasterClient;
        }

        public void SetIsMasterClient(bool value) {
            this.isMasterClient = value;
        }

        public void SetIsLocal(bool value) {
            this.isLocal = value;
        }

        public EntityType GetEntityType() {
            return this.type;
        }

        public void SetEntityType(EntityType type) {
            this.type = type;
        }
    }

}