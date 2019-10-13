using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace com.hermitGames.rp
{
    public class BuildUIManager : MonoBehaviour
    {
        [SerializeField] private Transform scrollviewContent;
        [SerializeField] private GameObject prefabButtonOption;

        public static BuildUIManager instance;

        // Start is called before the first frame update
        void Start() {
            instance = this;
        }

        public void Init() {
            foreach(GameObject prefab in GameManager.prefabDatabase.Values) {
                GameObject prefabOption = Instantiate(this.prefabButtonOption, this.scrollviewContent);
                prefabOption.GetComponentInChildren<TextMeshProUGUI>().text = prefab.name;
            }
        }
    }
}
