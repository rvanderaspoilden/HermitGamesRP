using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [RequireComponent(typeof(Camera))]
    public class CameraBuilding : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Color virtualColor;
        [SerializeField] private float speed = 30f;

        private new Camera camera;

        private float moveX;
        private float moveY;

        private GameObject selectedPrefab;
        private Color originalPrefabColor;

        // Start is called before the first frame update
        void Start() {
            this.camera = GetComponent<Camera>();

            this.selectedPrefab = Instantiate(this.prefab);
            this.selectedPrefab.SetActive(false);
            this.originalPrefabColor = this.selectedPrefab.GetComponent<Renderer>().material.color;
            this.selectedPrefab.GetComponent<Renderer>().material.color = this.virtualColor;
        }

        // Update is called once per frame
        void Update() {
            this.moveX = Input.GetAxis("Horizontal");
            this.moveY = Input.GetAxis("Vertical");

            Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast only on terrain
            if (Physics.Raycast(ray, out hit, 100, (1 << 8))) {
                if(this.selectedPrefab) {
                    this.selectedPrefab.SetActive(true);
                    this.selectedPrefab.transform.position = hit.point;

                    if (Input.GetMouseButtonDown(0)) {
                        this.selectedPrefab.GetComponent<Renderer>().material.color = this.originalPrefabColor;
                        this.selectedPrefab = null;
                    }
                }         
            } else if(this.selectedPrefab){
                this.selectedPrefab.SetActive(false);
            }
        }

        private void FixedUpdate() {
            this.transform.Translate(this.moveX * this.speed * Time.deltaTime, this.moveY * this.speed * Time.deltaTime, 0);
        }
    }
}
