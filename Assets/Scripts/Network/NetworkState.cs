using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkState : MonoBehaviour
    {
        private Dictionary<string, string> state = new Dictionary<string, string>();
        private NetworkIdentity networkIdentity;

        public delegate void StateChanged(Dictionary<string, string> state);
        public event StateChanged OnStateChanged;

        // Start is called before the first frame update
        void Start() {
            this.networkIdentity = GetComponent<NetworkIdentity>();
        }

        public void UpdateState(EntityState data) {
            this.state = data.entityState;
            OnStateChanged(this.state);
        }

        public void Synchronize(Dictionary<string, string> newState) {
            EntityState entityState = new EntityState();
            entityState.entityId = this.networkIdentity.GetNetworkID();
            entityState.entityType = this.networkIdentity.GetEntityType();
            entityState.entityState = newState;

            NetworkManager.instance.GetSocket().Emit("Packet::StateChanged", JSONObject.Create(JsonConvert.SerializeObject(entityState)), (JSONObject data) => {
                this.state = newState;
            }) ;
        }
    }
}