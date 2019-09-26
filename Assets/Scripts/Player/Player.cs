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

        // Start is called before the first frame update
        void Start()
        {
            this.characterController = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (characterController.isGrounded)
            {
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
                moveDirection = this.transform.TransformDirection(moveDirection);
                moveDirection *= speed;

                if (Input.GetButton("Jump"))
                {
                    moveDirection.y = jumpSpeed;
                }
            }

            moveDirection.y -= gravity * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);

            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
    }
}