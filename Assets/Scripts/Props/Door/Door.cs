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

        [SerializeField] private Quaternion backwardRotation;
        [SerializeField] private Quaternion forwardRotation;

        private float speed = 2f;

        private NetworkState networkState;
        private NetworkIdentity networkIdentity;
        private NetworkAnimator networkAnimator;

        private bool isLoaded;

        private void Start() {
            this.networkState = GetComponent<NetworkState>();
            this.networkIdentity = GetComponent<NetworkIdentity>();
            this.networkAnimator = GetComponent<NetworkAnimator>();

            this.networkState.OnStateChanged += UpdateState;   
        }

        private void FixedUpdate() {
            if (this.isLoaded && GameManager.localPlayer.GetIsMasterClient()) {
                if (this.doorState == DoorState.OPENED) {
                    switch (this.direction) {
                        case DoorDirection.FORWARD:
                            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.forwardRotation, Time.deltaTime * this.speed);
                            break;
                        case DoorDirection.BACKWARD:
                            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.backwardRotation, Time.deltaTime * this.speed);
                            break;
                    }
                } else if (this.doorState == DoorState.CLOSED) {
                    this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.initialRotation, Time.deltaTime * this.speed);
                }
            }
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

            this.networkState.Synchronize(data);
        }

        public void UpdateState(Dictionary<string, string> state) {
            if(state != null) {
                int.TryParse(state["angleAmplitude"], out this.angleAmplitude);
                System.Enum.TryParse(state["direction"], out this.direction);
                System.Enum.TryParse(state["doorState"], out this.doorState);

                float x, y, z, w;

                float.TryParse(state["initialRotationX"], out x);
                float.TryParse(state["initialRotationY"], out y);
                float.TryParse(state["initialRotationZ"], out z);
                float.TryParse(state["initialRotationW"], out w);

                this.initialRotation = new Quaternion(x,y,z,w);

                this.forwardRotation = Quaternion.Euler(0, this.initialRotation.eulerAngles.y + this.angleAmplitude, 0);
                this.backwardRotation = Quaternion.Euler(0, this.initialRotation.eulerAngles.y - this.angleAmplitude, 0);

                if (!this.isLoaded) {
                    this.isLoaded = true;
                }
            } else {
                this.initialRotation = this.transform.rotation;
                this.forwardRotation = Quaternion.Euler(0, this.initialRotation.eulerAngles.y + this.angleAmplitude, 0);
                this.backwardRotation = Quaternion.Euler(0, this.initialRotation.eulerAngles.y - this.angleAmplitude, 0);

                this.SetupState();

                if (!this.isLoaded) {
                    this.isLoaded = true;
                }
            }
        }

        public void OpenOrClose() {
            if (this.doorState == DoorState.CLOSED) {
                this.doorState = DoorState.OPENED;
                this.networkAnimator.SetAnimation(new SetAnimationRequest("open", VariableType.TRIGGER, ""));
                this.SetupState();

            } else if (this.doorState == DoorState.OPENED) {
                this.doorState = DoorState.CLOSED;
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
