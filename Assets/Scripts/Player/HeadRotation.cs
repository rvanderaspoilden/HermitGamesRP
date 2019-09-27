using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    public class HeadRotation : MonoBehaviour
    {
        public GameObject objectToSpawn;

        [SerializeField] private float sensitivityY = 15F;
        [SerializeField] private float minimumX = -360F;
        [SerializeField] private float maximumX = 360F;
        [SerializeField] private float minimumY = -60F;
        [SerializeField] private float maximumY = 60F;
        private float rotationY = 0F;

        private Transform cameraTransform;
        private Transform bodyTransform;

        // Start is called before the first frame update
        void Start()
        {
            this.cameraTransform = GetComponentInChildren<Camera>().transform;
            this.bodyTransform = GetComponentInParent<Player>().transform;
        }

        // Update is called once per frame
        void Update()
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY); 

            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 10f)) {
                if(hit.collider) {
                    if (Input.GetKeyDown(KeyCode.E)) {
                        GameManager.instance.CmdRegisterEntity(this.objectToSpawn, hit.point, Vector3.zero);
                    }

                    if (Input.GetKeyDown(KeyCode.R) && hit.collider.name.StartsWith(objectToSpawn.name)) {
                        GameManager.instance.CmdDestroyEntity(hit.collider.gameObject);
                    }
                }
            }
        }

        private void LateUpdate() {
            transform.localRotation = Quaternion.Euler(new Vector3(-rotationY, 0, 0));
            this.cameraTransform.rotation = Quaternion.Euler(new Vector3(transform.localEulerAngles.x, this.bodyTransform.eulerAngles.y, 0));
        }
    }
}
