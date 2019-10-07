using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [RequireComponent(typeof(Camera))]
    public class CameraBuilding : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private float unitCellSize = 2f;
        [SerializeField] private float moveSpeed = 30f;
        [SerializeField] private float zoomSpeed = 30f;

        private new Camera camera;

        private float moveX;
        private float moveY;
        private float moveZ;

        private GameObject selectedPrefab;
        private PreviewState previewState;

        // Start is called before the first frame update
        void Start() {
            this.camera = GetComponent<Camera>();  
        }

        private void OnEnable() {
            this.selectedPrefab = Instantiate(this.prefab);
            this.selectedPrefab.AddComponent<Preview>();
            this.selectedPrefab.SetActive(false);
            Preview.statusChangedEvent += this.PreviewStateChanged;
        }

        private void OnDisable() {
            Preview.statusChangedEvent -= this.PreviewStateChanged;
            Destroy(this.selectedPrefab);
        }

        private void PreviewStateChanged(PreviewState state) {
            Debug.Log(state);
            this.previewState = state;
        }

        // Update is called once per frame
        void Update() {
            this.moveX = Input.GetAxis("Horizontal");
            this.moveY = Input.GetAxis("Vertical");
            this.moveZ = Input.GetAxis("Mouse ScrollWheel");

            if (Input.GetMouseButtonDown(1)) {
                this.selectedPrefab.transform.Rotate(new Vector3(0, 90f, 0));
            }

            Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast only on terrain
            if (Physics.Raycast(ray, out hit, 100f)) {
                if (this.selectedPrefab) {
                    this.selectedPrefab.SetActive(true);
                    this.selectedPrefab.transform.position = this.GetNearestPoint(hit.point);

                    if (Input.GetMouseButtonDown(0)) {
                        GameManager.instance.CmdRegisterEntity(this.prefab, this.selectedPrefab.transform.position, this.selectedPrefab.transform.rotation.eulerAngles);
                    }
                }
            } else if (this.selectedPrefab) {
                this.selectedPrefab.SetActive(false);
            }
        }

        private void FixedUpdate() {
            this.transform.Translate(this.moveX * this.moveSpeed * Time.deltaTime, this.moveY * this.moveSpeed * Time.deltaTime, this.moveZ * this.zoomSpeed * Time.deltaTime);
        }

        private Vector3 GetNearestPoint(Vector3 pos) {
            return new Vector3(Mathf.RoundToInt(pos.x / this.unitCellSize) * this.unitCellSize, 0, Mathf.RoundToInt(pos.z / this.unitCellSize) * this.unitCellSize);
        }
    }
}
