using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    public class Preview : MonoBehaviour
    {
        public delegate void StatusChanged(PreviewState state);
        public static event StatusChanged statusChangedEvent;

        private Color virtualColorGreen = new Color(0, 1, 0, 0.4f);
        private Color virtualColorRed = new Color(1, 0, 0, 0.4f);
        private PreviewState state;

        // Start is called before the first frame update
        void Start() {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody == null) {
                rigidbody = this.gameObject.AddComponent<Rigidbody>();
            }

            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;

            foreach (Collider coll in GetComponentsInChildren<Collider>()) {
                coll.enabled = false;
            }

            foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
                renderer.material.color = this.virtualColorGreen;
            }
        }

        private void FixedUpdate() {
            RaycastHit hit;
            if (Physics.BoxCast(this.transform.position, transform.localScale, transform.forward, out hit, transform.rotation, 10f)) {
                Debug.Log("Collision forward");
            }

            if (Physics.BoxCast(this.transform.position, transform.localScale, -transform.forward, out hit, transform.rotation, 10f)) {
                Debug.Log("Collision backward");
            }

            if (Physics.BoxCast(this.transform.position, transform.localScale, Vector3.left, out hit, transform.rotation, 10f)) {
                Debug.Log("Collision left");
            }

            if (Physics.BoxCast(this.transform.position, transform.localScale, Vector3.right, out hit, transform.rotation, 10f)) {
                Debug.Log("Collision right");
            }
        }
    }

    public enum PreviewState
    {
        NOT_VALID,
        VALID
    }
}

