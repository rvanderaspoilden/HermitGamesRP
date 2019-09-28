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
        [SerializeField] private float jumpSpeed = 8.0f;
        [SerializeField] private float gravity = 20.0f;
        [SerializeField] private float sensitivityX = 15;

        private Vector3 moveDirection = Vector3.zero;

        private CharacterController characterController;

        private float forwardSpeed = 0f;

        private float isSprinting = 0f;

        private NetworkAnimator networkAnimator;

        // Start is called before the first frame update
        void Start() {
            this.characterController = GetComponent<CharacterController>();
            this.networkAnimator = GetComponent<NetworkAnimator>();
        }

        // Update is called once per frame
        void Update() {
            if (characterController.isGrounded) {
                this.isSprinting = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;

                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
                moveDirection = this.transform.TransformDirection(moveDirection);
                moveDirection *= speed * this.isSprinting;

                if (Input.GetButton("Jump")) {
                    moveDirection.y = jumpSpeed;
                }

                this.networkAnimator.SetAnimation(new SetAnimationRequest("moveSpeed", VariableType.FLOAT, (Input.GetAxis("Vertical") * this.isSprinting).ToString()));
            }

            moveDirection.y -= gravity * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);

            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
    }
}