using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [RequireComponent(typeof(NetworkAnimator))]
    [RequireComponent(typeof(NetworkTransform))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private float speed = 6.0f;
        [SerializeField] private float sprintAcceleratorCoef = 1.5f;
        [SerializeField] private float backwardDeceleratorCoef= 0.5f;
        [SerializeField] private float jumpSpeed = 8.0f;
        [SerializeField] private float gravity = 20.0f;
        [SerializeField] private float sensitivityX = 15;

        private Vector3 moveDirection = Vector3.zero;

        private CharacterController characterController;

        private bool isSprinting;

        private bool isWalkingBackward;

        private NetworkAnimator networkAnimator;

        // Start is called before the first frame update
        void Start() {
            this.characterController = GetComponent<CharacterController>();
            this.networkAnimator = GetComponent<NetworkAnimator>();
        }

        // Update is called once per frame
        void Update() {
            if (characterController.isGrounded) {
                this.isSprinting = Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0f;
                this.isWalkingBackward = Input.GetAxis("Vertical") < 0;

                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
                moveDirection = this.transform.TransformDirection(moveDirection);

                if(this.isSprinting) {
                    moveDirection *= (speed * this.sprintAcceleratorCoef);
                } else if(this.isWalkingBackward) {
                    moveDirection *= (speed * this.backwardDeceleratorCoef);
                } else {
                    moveDirection *= speed;
                }

                if (Input.GetButton("Jump")) {
                    moveDirection.y = jumpSpeed;
                    this.networkAnimator.SetAnimation(new SetAnimationRequest("jump", VariableType.TRIGGER, ""));
                }

                if (this.isSprinting) {
                    this.networkAnimator.SetAnimation(new SetAnimationRequest("moveY", VariableType.FLOAT, "2"));
                } else {
                    this.networkAnimator.SetAnimation(new SetAnimationRequest("moveY", VariableType.FLOAT, Input.GetAxis("Vertical").ToString()));
                    this.networkAnimator.SetAnimation(new SetAnimationRequest("moveX", VariableType.FLOAT, Input.GetAxis("Horizontal").ToString()));
                }
            }

            if(Input.GetMouseButton(1)) {
                this.networkAnimator.SetAnimation(new SetAnimationRequest("punchReady", VariableType.BOOL, bool.TrueString));
            } else {
                this.networkAnimator.SetAnimation(new SetAnimationRequest("punchReady", VariableType.BOOL, bool.FalseString));
            }

            if(Input.GetMouseButtonDown(0)) {
                this.networkAnimator.SetAnimation(new SetAnimationRequest("punch", VariableType.TRIGGER, ""));
            }

            moveDirection.y -= gravity * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);

            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
    }
}