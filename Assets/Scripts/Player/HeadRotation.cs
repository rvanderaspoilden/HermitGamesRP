using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    public class HeadRotation : MonoBehaviour
    {
        public GameObject objectToSpawn;

        public float sensitivityY = 15F;
        public float minimumX = -360F;
        public float maximumX = 360F;
        public float minimumY = -60F;
        public float maximumY = 60F;
        float rotationY = 0F;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, 0, 0);

            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 10f)) {
                if(hit.collider) {
                    if (Input.GetKeyDown(KeyCode.E)) {
                        GameManager.instance.CmdRegisterEntity(this.objectToSpawn, hit.point, Vector3.zero);
                    }

                    if (Input.GetKeyDown(KeyCode.R)) {
                        GameManager.instance.CmdDestroyEntity(hit.collider.gameObject);
                    }
                }
            }
        }
    }
}
