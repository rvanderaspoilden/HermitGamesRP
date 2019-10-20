using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(Animator))]
    public class NetworkAnimator : MonoBehaviour
    {
        private SetAnimationRequest previousAnimationRequest;

        private Animator animator;
        private NetworkIdentity identity;

        // Start is called before the first frame update
        void Start() {
            this.identity = GetComponent<NetworkIdentity>();
            this.animator = GetComponent<Animator>();
        }

        public void SetAnimation(SetAnimationRequest req) {
            switch(req.variableType) {
                case VariableType.BOOL:
                    this.animator.SetBool(req.variableName, bool.Parse(req.variableValue));
                    break; 
                case VariableType.FLOAT:
                    if(req.variableValue != "") 
                        this.animator.SetFloat(req.variableName, float.Parse(req.variableValue));
                    break;
                case VariableType.TRIGGER:
                    this.animator.SetTrigger(req.variableName);
                    break;
                case VariableType.INT:
                    this.animator.SetInteger(req.variableName, int.Parse(req.variableValue));
                    break;
            }

            if(this.identity.IsMine() && !req.isSameThan(this.previousAnimationRequest)) {
                this.CmdSyncronizeAnimation(req.variableName, req.variableType, req.variableValue);
                this.previousAnimationRequest = req;
            }
        }

        private void CmdSyncronizeAnimation(string name, VariableType type, string value) {
            SetAnimationRequest req = new SetAnimationRequest();
            req.entityId = this.identity.GetNetworkID();
            req.entityType = this.identity.GetEntityType();
            req.variableName = name;
            req.variableType = type;
            req.variableValue = value;

            NetworkManager.instance.GetSocket().Emit("Packet::SetAnimation", JSONObject.Create(JsonUtility.ToJson(req)));
        }
    }

    [System.Serializable]
    public class SetAnimationRequest
    {
        public string entityId;
        public EntityType entityType;
        public string variableName;
        public VariableType variableType;
        public string variableValue;

        public SetAnimationRequest() { }
       
        public SetAnimationRequest(string name, VariableType type, string value) {
            this.variableName = name;
            this.variableType = type;
            this.variableValue = value;
        }

        public bool isSameThan(SetAnimationRequest req) {
            return req != null && this.variableName == req.variableName && this.variableType == req.variableType && this.variableValue == req.variableValue;
        }
    }

    [System.Serializable]
    public enum VariableType
    {
        FLOAT,
        BOOL,
        TRIGGER,
        INT
    }
}
