using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(NetworkState))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(NetworkAnimator))]
    public class Door : MonoBehaviour
    {
        [SerializeField] private int angleAmplitude = 90;
        [SerializeField] private DoorDirection direction = DoorDirection.FORWARD;
        [SerializeField] private DoorState doorState = DoorState.CLOSED;
        [SerializeField] private Quaternion initialRotation;
        //[SerializeField] private LockState lockState = LockState.UNLOCKED;

        private Quaternion targetRotation;

        private float speed = 2f;

        private NetworkState networkState;
        private NetworkIdentity networkIdentity;
        private NetworkAnimator networkAnimator;
 
        private void Start() {
            this.networkState = GetComponent<NetworkState>();
            this.networkIdentity = GetComponent<NetworkIdentity>();
            this.networkAnimator = GetComponent<NetworkAnimator>();

            this.networkState.OnStateChanged += UpdateState;

            this.initialRotation = this.transform.rotation;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.E)) {
                this.OpenOrClose();
            }

            if(Input.GetKeyDown(KeyCode.U)) {
                this.SetupState();
            } 
        }

        private void FixedUpdate() {
          this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.targetRotation, Time.deltaTime * this.speed);
        }

        private void SetupState() {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("angleAmplitude", this.angleAmplitude.ToString());
            data.Add("direction", this.direction.ToString());
            data.Add("doorState", this.doorState.ToString());
            data.Add("initialRotationX", this.initialRotation.x.ToString());
            data.Add("initialRotationY", this.initialRotation.y.ToString());
            data.Add("initialRotationZ", this.initialRotation.z.ToString());
            data.Add("initialRotationW", this.initialRotation.w.ToString());
            data.Add("targetRotationY", this.targetRotation.y.ToString());

            this.networkState.Synchronize(data);
            Debug.Log("SAVED MODIFICATION");
        }

        public void UpdateState(Dictionary<string, string> state) {
            int.TryParse(state["angleAmplitude"], out this.angleAmplitude);
            System.Enum.TryParse(state["direction"], out this.direction);
            System.Enum.TryParse(state["doorState"], out this.doorState);
            float.TryParse(state["initialRotationX"], out this.initialRotation.x);
            float.TryParse(state["initialRotationY"], out this.initialRotation.y);
            float.TryParse(state["initialRotationZ"], out this.initialRotation.z);
            float.TryParse(state["initialRotationW"], out this.initialRotation.w);
            float.TryParse(state["targetRotationY"], out this.targetRotation.y);

            Debug.Log("Get existing data");
        }

        public void OpenOrClose() {
            if (this.doorState == DoorState.CLOSED) {
                this.doorState = DoorState.OPENED;

                float y = this.transform.rotation.eulerAngles.y;

                switch(this.direction) {
                    case DoorDirection.FORWARD:
                        y += this.angleAmplitude;
                        break;
                    case DoorDirection.BACKWARD:
                        y -= this.angleAmplitude;
                        break;
                }

                this.targetRotation = Quaternion.Euler(0, y, 0);

                this.networkAnimator.SetAnimation(new SetAnimationRequest("open", VariableType.TRIGGER, ""));
                this.SetupState();

            } else if (this.doorState == DoorState.OPENED) {
                this.doorState = DoorState.CLOSED;
                this.targetRotation = this.initialRotation;
                this.SetupState();
            }
        }
    }

    public enum DoorState
    {
        OPENED,
        CLOSED
    }

    public enum DoorDirection
    {
        FORWARD,
        BACKWARD,
        BOTH
    }

    public enum LockState
    {
        LOCKED,
        UNLOCKED
    }
}
