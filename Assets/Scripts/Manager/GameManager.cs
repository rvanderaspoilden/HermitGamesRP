﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace com.hermitGames.rp
{
    public class GameManager : MonoBehaviour
    {
        private Dictionary<string, NetworkIdentity> networkIdentities = new Dictionary<string, NetworkIdentity>();
        private Dictionary<string, NetworkAnimator> networkAnimators = new Dictionary<string, NetworkAnimator>();
        private Dictionary<string, NetworkTransform> networkTransforms = new Dictionary<string, NetworkTransform>();
        private Dictionary<string, NetworkState> networkStates = new Dictionary<string, NetworkState>();
        private Dictionary<string, NetworkVoice> networkVoices = new Dictionary<string, NetworkVoice>();

        public static Dictionary<string, GameObject> prefabDatabase = new Dictionary<string, GameObject>();

        private CameraBuilding cameraBuilding;
        private bool isBuildingMode;

        private BuildUIManager buildUIManager;

        private GameObject localPlayer;

        public static GameManager instance;

        // Start is called before the first frame update
        void Start() {
            this.cameraBuilding = GameObject.FindObjectOfType<CameraBuilding>();
            this.cameraBuilding.gameObject.SetActive(false);

            this.SetPrefabDatabase();

            this.buildUIManager = GameObject.FindObjectOfType<BuildUIManager>();
            this.buildUIManager.Init();
            this.buildUIManager.gameObject.SetActive(false);

            instance = this;

            this.InitEntities();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.B)) {
                this.isBuildingMode = !this.isBuildingMode;

                if (this.isBuildingMode) {
                    this.SwitchToBuildMode();
                } else {
                    this.SwitchToPlayerMode();
                }
            }
        }

        public void SwitchToBuildMode() {
            this.buildUIManager.gameObject.SetActive(true);
            this.cameraBuilding.gameObject.SetActive(true);
            this.cameraBuilding.transform.position = new Vector3(this.localPlayer.transform.position.x, 20, this.localPlayer.transform.position.z);
            this.localPlayer.GetComponentInChildren<Camera>().enabled = false;
            this.localPlayer.GetComponent<Player>().enabled = false;
            this.localPlayer.GetComponent<PlayerRotation>().enabled = false;
        }

        public void SwitchToPlayerMode() {
            this.localPlayer.GetComponentInChildren<Camera>().enabled = true;
            this.localPlayer.GetComponent<Player>().enabled = true;
            this.localPlayer.GetComponent<PlayerRotation>().enabled = true;
            this.cameraBuilding.gameObject.SetActive(false);
            this.buildUIManager.gameObject.SetActive(false);
        }

        public void EntityMoved(string id, Vector3 newPosition) {
            this.networkTransforms[id].UpdatePosition(newPosition);
        }

        public void EntityDoAnimation(SetAnimationRequest req) {
            this.networkAnimators[req.entityId].SetAnimation(req);
        }

        public void EntityRotated(string id, Vector3 newPosition) {
            this.networkTransforms[id].UpdateRotation(newPosition);
        }

        public void EntityTalk(VoicePacket packet) {
            this.networkVoices[packet.entityId].PlayVoiceSound(packet.data, packet.channels);
        }

        public void EntityStateChanged(EntityState state) {
            this.networkStates[state.entityId].UpdateState(state.entityState);
        }

        public void CmdRegisterEntity(GameObject prefab, Vector3 position, Vector3 rotation) {
            Entity entity = new Entity();
            entity.type = prefab.GetComponent<NetworkIdentity>().GetEntityType();
            entity.position = new Vector3Json(position);
            entity.rotation = new Vector3Json(rotation);
            entity.prefabName = prefab.name;

            NetworkManager.instance.GetSocket().Emit("Packet::RegisterEntity", JSONObject.Create(JsonUtility.ToJson(entity)));
        }

        public void CmdDestroyEntity(GameObject target) {
            NetworkIdentity networkIdentity = target.GetComponent<NetworkIdentity>();
            Entity entity = new Entity();
            entity.id = networkIdentity.GetNetworkID();
            entity.type = networkIdentity.GetEntityType();

            NetworkManager.instance.GetSocket().Emit("Packet::DestroyEntity", JSONObject.Create(JsonUtility.ToJson(entity)));
        }

        public GameObject InstantiateEntity(Entity entity, bool isMine) {
            Vector3 position;
            float.TryParse(entity.position.x, out position.x);
            float.TryParse(entity.position.y, out position.y);
            float.TryParse(entity.position.z, out position.z);

            Vector3 rotation;
            float.TryParse(entity.rotation.x, out rotation.x);
            float.TryParse(entity.rotation.y, out rotation.y);
            float.TryParse(entity.rotation.z, out rotation.z);

            GameObject entityObject = Instantiate(prefabDatabase[entity.prefabName], position, Quaternion.Euler(rotation));

            NetworkIdentity networkIdentity = entityObject.GetComponent<NetworkIdentity>();
            networkIdentity.Setup(entity.id, isMine);
            this.networkIdentities.Add(entity.id, networkIdentity);

            NetworkAnimator networkAnimator = entityObject.GetComponent<NetworkAnimator>();

            if (networkAnimator != null) {
                this.networkAnimators.Add(entity.id, networkAnimator);
            }

            NetworkTransform networkTransform = entityObject.GetComponent<NetworkTransform>();

            if (networkTransform != null) {
                this.networkTransforms.Add(entity.id, networkTransform);
            }

            NetworkState networkState = entityObject.GetComponent<NetworkState>();

            if (networkState != null) {
                this.networkStates.Add(entity.id, networkState);
            }

            if (entity.id != NetworkManager.instance.GetLocalUser().id) {
                // Get current state => like it because can't deserialize state in user from networkManager
                if (networkState != null) {
                    NetworkManager.instance.GetSocket().Emit("Packet::GetEntityState", JSONObject.Create(JsonUtility.ToJson(entity)));
                }

                if (networkAnimator != null && entity.animation != null && entity.animation.variableName != null) {
                    StartCoroutine(this.PlayAnimationLater(entity));
                }
            }

            return entityObject;
        }

        public IEnumerator PlayAnimationLater(Entity entity) {
            yield return new WaitForSeconds(0.1f);
            this.networkAnimators[entity.id].SetAnimation(entity.animation);
        }

        public GameObject InstantiatePlayer(User user, bool isMine) {
            GameObject player = this.InstantiateEntity(user, isMine);

            this.networkVoices.Add(user.id, player.GetComponent<NetworkVoice>());

            if (!isMine) {
                player.GetComponent<Player>().enabled = false;
                player.GetComponent<CharacterController>().enabled = false;
                player.GetComponentInChildren<Camera>().enabled = false;
                player.GetComponentInChildren<AudioListener>().enabled = false;
            } else {
                this.localPlayer = player;
            }

            return player;
        }

        public void DestroyEntity(Entity entity) {
            Destroy(this.networkIdentities[entity.id].gameObject);
            Destroy(this.networkAnimators[entity.id].gameObject);
            Destroy(this.networkTransforms[entity.id].gameObject);
            Destroy(this.networkStates[entity.id].gameObject);

            if (entity.type == EntityType.USER)
                Destroy(this.networkVoices[entity.id].gameObject);
        }

        private void SetPrefabDatabase() {
            prefabDatabase = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs"))
                .FindAll((GameObject obj) => obj.GetComponentInChildren<NetworkIdentity>())
                .ToDictionary((GameObject prefab) => prefab.name);
        }

        private void InitEntities() {
            foreach (Entity entity in NetworkManager.instance.GetEntities().buildings) {
                this.InstantiateEntity(entity, NetworkManager.instance.GetLocalUser().id == entity.ownerId);
            }

            foreach (Entity entity in NetworkManager.instance.GetEntities().roads) {
                this.InstantiateEntity(entity, NetworkManager.instance.GetLocalUser().id == entity.ownerId);
            }

            foreach (Entity entity in NetworkManager.instance.GetEntities().props) {
                this.InstantiateEntity(entity, NetworkManager.instance.GetLocalUser().id == entity.ownerId);
            }

            foreach (User user in NetworkManager.instance.GetEntities().users) {
                this.InstantiatePlayer(user, NetworkManager.instance.GetLocalUser().id == user.id);
            }
        }
    }
}
