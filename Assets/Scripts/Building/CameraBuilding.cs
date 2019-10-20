using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        public static CameraBuilding instance;

        // Start is called before the first frame update
        void Start() {
            this.camera = GetComponent<Camera>();
            instance = this;
        }

        private void OnEnable() {
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

        public void SetPrefab(string name) {
            GameObject prefabFound = GameManager.prefabDatabase[name];

            if (prefabFound) {
                Destroy(this.selectedPrefab);

                this.prefab = prefabFound;
                this.selectedPrefab = Instantiate(this.prefab);
                this.selectedPrefab.name = this.prefab.name;
                this.selectedPrefab.AddComponent<Preview>();
            } else {
                Debug.LogErrorFormat("Prefab : {0} not found in database", name);
            }
        }

        // Update is called once per frame
        void Update() {
            if (EventSystem.current.IsPointerOverGameObject()) {
                if (this.selectedPrefab) {
                    this.selectedPrefab.SetActive(false);
                }
                return;
            }

            this.moveX = Input.GetAxis("Horizontal");
            this.moveY = Input.GetAxis("Vertical");
            this.moveZ = Input.GetAxis("Mouse ScrollWheel");

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Destroy(this.selectedPrefab);
            }

            if (Input.GetMouseButtonDown(1)) {
                this.selectedPrefab.transform.Rotate(new Vector3(0, 90f, 0));
            }

            Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast only on terrain
            if (Physics.Raycast(ray, out hit, 100f)) {
                if (this.selectedPrefab) {
                    if(hit.collider.tag == "Terrain" || (hit.collider.transform.parent && hit.collider.transform.parent.gameObject == this.selectedPrefab)) {
                        this.selectedPrefab.SetActive(true);
                        this.selectedPrefab.transform.position = this.GetNearestPoint(hit.point);

                        if (Input.GetMouseButtonDown(0)) {
                            List<NetworkIdentity> networkIdentities = new List<NetworkIdentity>(this.selectedPrefab.GetComponentsInChildren<NetworkIdentity>());

                            Debug.Log(networkIdentities.Count);

                            networkIdentities.ForEach((NetworkIdentity identity) => {
                                if(GameManager.prefabDatabase.ContainsKey(identity.name))
                                    GameManager.instance.CmdRegisterEntity(GameManager.prefabDatabase[identity.name], identity.transform.position, identity.transform.rotation.eulerAngles);
                            });
                        }
                    } else {
                        this.selectedPrefab.SetActive(false);
                    }               
                }

                if (Input.GetKeyDown(KeyCode.R) && !this.selectedPrefab) {
                    NetworkIdentity networkIdentity = hit.transform.GetComponentInParent<NetworkIdentity>();

                    if (networkIdentity) {
                        GameManager.instance.CmdDestroyEntity(networkIdentity.gameObject);
                    } else {
                        Debug.LogError("No network identity found !");
                    }
                }
            }
        }

        private void FixedUpdate() {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            this.transform.Translate(this.moveX * this.moveSpeed * Time.deltaTime, this.moveY * this.moveSpeed * Time.deltaTime, this.moveZ * this.zoomSpeed * Time.deltaTime);
        }

        private Vector3 GetNearestPoint(Vector3 pos) {
            return new Vector3(Mathf.RoundToInt(pos.x / this.unitCellSize) * this.unitCellSize, 0, Mathf.RoundToInt(pos.z / this.unitCellSize) * this.unitCellSize);
        }

    }
}
