using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(NetworkState))]
    [RequireComponent(typeof(NetworkIdentity))]

    public class PlayerRotation : MonoBehaviour
    {
        [SerializeField] private Transform headBone;

        [SerializeField] private float sensitivityX = 5f;
        [SerializeField] private float sensitivityY = 5f;

        [SerializeField] private float minimumY = -55f;
        [SerializeField] private float maximumY = 60f;

        private float rotationY = 0f;
        private float previousRotationY = 0f;

        private Transform cameraTransform;
        private NetworkState networkState;
        private NetworkIdentity networkIdentity;

        // Start is called before the first frame update
        void Start() {
            this.networkState = GetComponent<NetworkState>();
            this.networkIdentity = GetComponent<NetworkIdentity>();
            this.cameraTransform = GetComponentInChildren<Camera>().transform;

            this.networkState.OnStateChanged += UpdateState;
        }

        // Update is called once per frame
        void Update() {
            if(this.networkIdentity.IsMine()) {
                this.ManageVerticalRotation();
                this.ManageHorizontalRotation();
            }
        }

        public void UpdateState(Dictionary<string, string> state) {
            float.TryParse(state["rotationY"], out this.rotationY);
        }

        private void LateUpdate() {
            this.headBone.localRotation = Quaternion.Euler(new Vector3(-rotationY, 0, 0));
            this.cameraTransform.rotation = Quaternion.Euler(new Vector3(this.headBone.localEulerAngles.x, this.transform.eulerAngles.y, 0));
        }

        private void ManageHorizontalRotation() {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }

        private void ManageVerticalRotation() {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            if(Mathf.Abs(previousRotationY - rotationY) >= 1f) {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("rotationY", rotationY.ToString());
                data.Add("rotationX", rotationY.ToString());

                this.networkState.Synchronize(data);
                this.previousRotationY = rotationY;
            }
        }
    }
}
