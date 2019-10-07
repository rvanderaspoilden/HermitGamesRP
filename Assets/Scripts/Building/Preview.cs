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
        private new Renderer renderer;
        private PreviewState state;

        // Start is called before the first frame update
        void Start() {
            this.renderer = GetComponent<Renderer>();

            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody == null) {
                rigidbody = this.gameObject.AddComponent<Rigidbody>();
            }

            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;

            this.renderer.material.color = this.virtualColorGreen;

            foreach (Collider coll in GetComponentsInChildren<Collider>()) {
                coll.isTrigger = true;
            }
        }

        private void OnTriggerStay(Collider other) {
            if (other.gameObject.name != "Terrain") {
                if (this.state == PreviewState.VALID) {
                    this.state = PreviewState.NOT_VALID;
                    statusChangedEvent(this.state);
                    this.renderer.material.color = this.virtualColorRed;
                }
            }
        }

        private void OnTriggerExit(Collider other) {
            if (state == PreviewState.NOT_VALID) {
                this.state = PreviewState.VALID;
                statusChangedEvent(this.state);
                this.renderer.material.color = this.virtualColorGreen;
            }
        }
    }

    public enum PreviewState
    {
        NOT_VALID,
        VALID
    }
}

