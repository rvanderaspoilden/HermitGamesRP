using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkTransform : MonoBehaviour
    {
        public bool syncPosition = true;
        public bool syncRotation = true;

        private Vector3 oldPosition;
        private float positionTimer;

        private Quaternion oldRotation;
        private float rotationTimer;

        private NetworkIdentity identity;

        // Start is called before the first frame update
        void Start() {
            this.identity = GetComponent<NetworkIdentity>();
        }

        // Update is called once per frame
        void Update() {
            if(this.identity.IsMine()) {
                if(this.syncPosition) {
                    if (this.oldPosition != this.transform.position) {
                        this.positionTimer = 0f;
                        this.oldPosition = this.transform.position;
                        this.SyncronizePosition();
                    } else {
                        this.positionTimer += Time.deltaTime;

                        if (this.positionTimer >= 5f) {
                            this.positionTimer = 0f;
                            this.SyncronizePosition();
                        }
                    }
                }

                if (this.syncRotation) {
                    if (this.oldRotation != this.transform.rotation) {
                        this.rotationTimer = 0f;
                        this.oldRotation = this.transform.rotation;
                        this.SyncronizeRotation();
                    } else {
                        this.rotationTimer += Time.deltaTime;

                        if (this.rotationTimer >= 5f) {
                            this.rotationTimer = 0f;
                            this.SyncronizeRotation();
                        }
                    }
                }
            }
        }

        public void UpdatePosition(Vector3 pos) {
            this.transform.position = pos;
        }

        public void UpdateRotation(Vector3 rot) {
            this.transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
        }

        private void SyncronizePosition() {
            UpdatePositionRequest req = new UpdatePositionRequest(this.identity.GetNetworkID(), this.identity.GetEntityType(), this.transform.position);

            NetworkManager.instance.GetSocket().Emit("Packet::UpdatePositionRequest", JSONObject.Create(JsonUtility.ToJson(req)));
        }

        private void SyncronizeRotation() {
            UpdateRotationRequest req = new UpdateRotationRequest(this.identity.GetNetworkID(), this.identity.GetEntityType(), this.transform.rotation.eulerAngles);

            NetworkManager.instance.GetSocket().Emit("Packet::UpdateRotationRequest", JSONObject.Create(JsonUtility.ToJson(req)));
        }
    }

    [System.Serializable]
    public class UpdatePositionRequest
    {
        public string entityId;
        public EntityType entityType;
        public string x;
        public string y;
        public string z;

        public UpdatePositionRequest(string entityId, EntityType entityType, Vector3 newPosition) {
            this.entityId = entityId;
            this.entityType = entityType;
            this.x = newPosition.x.ToString();
            this.y = newPosition.y.ToString();
            this.z = newPosition.z.ToString();
        }
    }

    [System.Serializable]
    public class UpdateRotationRequest
    {
        public string entityId;
        public EntityType entityType;
        public string x;
        public string y;
        public string z;

        public UpdateRotationRequest(string entityId, EntityType entityType, Vector3 newRotation) {
            this.entityId = entityId;
            this.entityType = entityType;
            this.x = newRotation.x.ToString();
            this.y = newRotation.y.ToString();
            this.z = newRotation.z.ToString();
        }
    }
}
