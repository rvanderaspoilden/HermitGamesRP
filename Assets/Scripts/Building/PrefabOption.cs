using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace com.hermitGames.rp
{
    [RequireComponent(typeof(Button))]
    public class PrefabOption : MonoBehaviour
    {
        void Start() {
            GetComponent<Button>().onClick.AddListener(() => {
                CameraBuilding.instance.SetPrefab(gameObject.GetComponentInChildren<TextMeshProUGUI>().text);
            });
        }
    }
}
