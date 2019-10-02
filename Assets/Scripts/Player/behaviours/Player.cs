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
        [SerializeField] private float backwardDeceleratorCoef = 0.5f;
        [SerializeField] private float jumpSpeed = 8.0f;
        [SerializeField] private float gravity = 20.0f;

        [SerializeField] private PlayerStateEnum curStateLowerBody = PlayerStateEnum.IDLE;
        [SerializeField] private PlayerStateEnum curStateUpperBody = PlayerStateEnum.IDLE;

        private Vector3 moveDirection = Vector3.zero;

        private CharacterController characterController;
        private NetworkAnimator networkAnimator;
        private NetworkState networkState;

        private float previousMoveX = 0f;
        private float previousMoveY = 0f;

        // Start is called before the first frame update
        void Start() {
            this.characterController = GetComponent<CharacterController>();
            this.networkAnimator = GetComponent<NetworkAnimator>();
        }

        // Update is called once per frame
        void Update() {
            if (characterController.isGrounded) {
                float verticalAxis = Mathf.Round(Input.GetAxis("Vertical") * 100f) / 100f;
                float horizontalAxis = Mathf.Round(Input.GetAxis("Horizontal") * 100f) / 100f;

                moveDirection = new Vector3(horizontalAxis, 0.0f, verticalAxis);
                moveDirection = this.transform.TransformDirection(moveDirection);

                if (Input.GetKey(KeyCode.LeftShift) && verticalAxis > 0f) {
                    moveDirection *= (speed * this.sprintAcceleratorCoef);
                } else if (verticalAxis < 0f) {
                    moveDirection *= (speed * this.backwardDeceleratorCoef);
                } else {
                    moveDirection *= speed;
                }

                if (Input.GetButton("Jump")) {
                    moveDirection.y = jumpSpeed;
                    this.curStateLowerBody = PlayerStateEnum.JUMP;
                    this.networkAnimator.SetAnimation(new SetAnimationRequest("jump", VariableType.TRIGGER, ""));
                }

                if (Input.GetKey(KeyCode.LeftShift) && verticalAxis > 0f) {
                    this.curStateLowerBody = PlayerStateEnum.MOVE;
                    if (this.previousMoveY != 2f) {
                        this.networkAnimator.SetAnimation(new SetAnimationRequest("moveY", VariableType.FLOAT, "2"));
                        this.previousMoveY = 2f;
                    }
                } else if (moveDirection.magnitude > 0f) {
                    this.curStateLowerBody = PlayerStateEnum.MOVE;

                    if (this.previousMoveY != verticalAxis) {
                        this.networkAnimator.SetAnimation(new SetAnimationRequest("moveY", VariableType.FLOAT, verticalAxis.ToString()));
                        this.previousMoveY = verticalAxis;
                    }

                    if (this.previousMoveX != horizontalAxis) {
                        this.networkAnimator.SetAnimation(new SetAnimationRequest("moveX", VariableType.FLOAT, horizontalAxis.ToString()));
                        this.previousMoveX = horizontalAxis;
                    }
                } else {
                    if (this.curStateLowerBody == PlayerStateEnum.MOVE) {
                        this.networkAnimator.SetAnimation(new SetAnimationRequest("moveY", VariableType.FLOAT, "0"));
                        this.networkAnimator.SetAnimation(new SetAnimationRequest("moveX", VariableType.FLOAT, "0"));
                    }
                    this.curStateLowerBody = PlayerStateEnum.IDLE;
                }
            }

            if (Input.GetMouseButton(1)) {
                this.curStateUpperBody = PlayerStateEnum.READY_TO_PUNCH;
                this.networkAnimator.SetAnimation(new SetAnimationRequest("punchReady", VariableType.BOOL, bool.TrueString));
            } else if (this.curStateUpperBody == PlayerStateEnum.READY_TO_PUNCH) {
                this.curStateUpperBody = PlayerStateEnum.IDLE;
                this.networkAnimator.SetAnimation(new SetAnimationRequest("punchReady", VariableType.BOOL, bool.FalseString));
            }

            if (Input.GetMouseButtonDown(0)) {
                this.curStateUpperBody = PlayerStateEnum.PUNCH;
                this.networkAnimator.SetAnimation(new SetAnimationRequest("punch", VariableType.TRIGGER, ""));
            }

            moveDirection.y -= gravity * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);
        }
    }
}