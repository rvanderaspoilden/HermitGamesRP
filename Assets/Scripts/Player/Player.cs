using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
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

        private Animator animator;

        // Start is called before the first frame update
        void Start() {
            this.characterController = GetComponent<CharacterController>();
            this.animator = GetComponent<Animator>();
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

                this.animator.SetFloat("moveSpeed", Input.GetAxis("Vertical") * this.isSprinting);
            }

            moveDirection.y -= gravity * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);

            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
    }
}