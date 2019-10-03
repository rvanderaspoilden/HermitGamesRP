using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
using System.Linq;
using Newtonsoft.Json;

namespace com.hermitGames.rp
{
    public class NetworkManager : MonoBehaviour
    {
        private SocketIOComponent socketIO;

        [SerializeField] private User localUser;

        [SerializeField] private EntityDatabase entities;

        public static NetworkManager instance;

        // Start is called before the first frame update
        void Start() {
            this.socketIO = GetComponent<SocketIOComponent>();
            this.socketIO.On("disconnect", this.CloseEvent);
            this.socketIO.On("Packet::InitGame", this.InitGame);
            this.socketIO.On("Packet::OtherPlayerJoined", this.OtherPlayerJoined);
            this.socketIO.On("Packet::OtherPlayerLeft", this.OtherPlayerLeft);
            this.socketIO.On("Packet::EntityMove", this.EntityMove);
            this.socketIO.On("Packet::EntityRotate", this.EntityRotate);
            this.socketIO.On("Packet::EntityCreated", this.EntityCreated);
            this.socketIO.On("Packet::EntityDestroyed", this.EntityDestroyed);
            this.socketIO.On("Packet::EntityDoAnimation", this.EntityDoAnimation);
            this.socketIO.On("Packet::EntityTalk", this.EntityTalk);
            this.socketIO.On("Packet::EntityStateChanged", this.EntityStateChanged);
        }


        void Awake() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            } else {
                Destroy(this.gameObject);
            }
        }

        // Update is called once per frame
        void Update() {

        }

        public User GetLocalUser() {
            return this.localUser;
        }

        public EntityDatabase GetEntities() {
            return this.entities;
        }

        public SocketIOComponent GetSocket() {
            return this.socketIO;
        }

        public void Join() {
            if (this.socketIO.IsConnected) {
                this.socketIO.Emit("Packet::JoinRequest");
            } else {
                Debug.LogError("Player is not connected to server");
            }
        }

        private void CloseEvent(SocketIOEvent e) {
            Debug.LogError("Connection with server lost");
            this.localUser = null;
            this.entities = null;
            SceneManager.LoadScene("Launcher");
        }

        private void EntityTalk(SocketIOEvent e) {
            VoicePacket packet = JsonUtility.FromJson<VoicePacket>(e.data.ToString());
            GameManager.instance.EntityTalk(packet);
        }

        private void EntityStateChanged(SocketIOEvent e) {
            EntityState state = JsonConvert.DeserializeObject<EntityState>(e.data.ToString());
            GameManager.instance.EntityStateChanged(state);
        }

        private void InitGame(SocketIOEvent e) {
            User localUser = JsonUtility.FromJson<User>(e.data[0].ToString());
            EntityDatabase entities = JsonUtility.FromJson<EntityDatabase>(e.data[1].ToString());

            this.localUser = localUser;
            this.entities = entities;

            SceneManager.LoadScene("InGame");
        }

        private void EntityMove(SocketIOEvent e) {
            UpdatePositionRequest req = JsonUtility.FromJson<UpdatePositionRequest>(e.data.ToString());
            GameManager.instance.EntityMoved(req.entityId, new Vector3(float.Parse(req.x), float.Parse(req.y), float.Parse(req.z)));
        }

        private void EntityDoAnimation(SocketIOEvent e) {
            SetAnimationRequest req = JsonUtility.FromJson<SetAnimationRequest>(e.data.ToString());
            GameManager.instance.EntityDoAnimation(req);
        }

        private void EntityRotate(SocketIOEvent e) {
            UpdateRotationRequest req = JsonUtility.FromJson<UpdateRotationRequest>(e.data.ToString());
            GameManager.instance.EntityRotated(req.entityId, new Vector3(float.Parse(req.x), float.Parse(req.y), float.Parse(req.z)));
        }

        private void EntityCreated(SocketIOEvent e) {
            Entity entity = JsonUtility.FromJson<Entity>(e.data.ToString());
            GameManager.instance.InstantiateEntity(entity, entity.ownerId == this.localUser.uid);
        }

        private void EntityDestroyed(SocketIOEvent e) {
            Entity entity = JsonUtility.FromJson<Entity>(e.data.ToString());
            GameManager.instance.DestroyEntity(entity);
        }

        private void OtherPlayerJoined(SocketIOEvent e) {
            User newUser = JsonUtility.FromJson<User>(e.data[0].ToString());
            this.entities.users = JsonUtility.FromJson<EntityDatabase>(e.data[1].ToString()).users;
            GameManager.instance.InstantiatePlayer(newUser, false);
        }

        private void OtherPlayerLeft(SocketIOEvent e) {
            User user = JsonUtility.FromJson<User>(e.data.ToString());
            GameManager.instance.DestroyEntity(user);
        }
    }
}
