using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    public class PlayerInteraction : MonoBehaviour
    {
        private new Camera camera;

        // Start is called before the first frame update
        void Start() {
            this.camera = GetComponentInChildren<Camera>();
        }

        // Update is called once per frame
        void Update() {
            RaycastHit hit;
            Debug.DrawRay(this.camera.transform.position, this.camera.transform.forward, Color.red);
            if (Physics.Raycast(this.camera.transform.position, this.camera.transform.forward, out hit, 10f)) {
                if (hit.collider.tag == "Door" && Input.GetKeyDown(KeyCode.E)) {
                    hit.collider.GetComponentInParent<Door>().OpenOrClose();
                }
            }
        }
    }
}
