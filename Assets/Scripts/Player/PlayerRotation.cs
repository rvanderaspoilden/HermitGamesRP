using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [RequireComponent(typeof(Player))]
    public class PlayerRotation : MonoBehaviour
    {
        [SerializeField] private Transform headBone;

        [SerializeField] private float sensitivityX = 5f;
        [SerializeField] private float sensitivityY = 5f;

        [SerializeField] private float minimumY = -55f;
        [SerializeField] private float maximumY = 60f;

        private float rotationY = 0f;

        private Transform cameraTransform;

        // Start is called before the first frame update
        void Start() {
            this.cameraTransform = GetComponentInChildren<Camera>().transform;
        }

        // Update is called once per frame
        void Update() {
            this.ManageVerticalRotation();
            this.ManageHorizontalRotation();
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
        }
    }
}
