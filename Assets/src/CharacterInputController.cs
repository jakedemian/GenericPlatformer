using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputController : MonoBehaviour {
    private bool wallJumpingEnabled = true;
    private bool airJumpsEnabled = true;

    private CharacterPhysicsController physics;
    private int airJumpsLeft = 0;

    private const float HORIZONTAL_GROUND_SPEED = 7f;
    private const float HORIZONTAL_AIR_SPEED = 9f;
    private const float JUMP_VELOCITY = 10f;
    private const float AIR_JUMP_VELOCITY = 8f;
    private const int MAX_AIR_JUMPS = 1;

    // INPUT LOCKS
    private float inputLockTimer = 0f;
    private const float WALL_JUMP_INPUT_LOCK = 0.2f;

    // Use this for initialization
    void Start() {
        physics = GetComponent<CharacterPhysicsController>();
    }

    /// <summary>
    ///  FIXED UPDATE
    /// </summary>
    void FixedUpdate() {
        if(inputLockTimer > 0f) {
            inputLockTimer = inputLockTimer - Time.deltaTime <= 0f ? 0f : inputLockTimer - Time.deltaTime;           
        }
    }

    // Update is called once per frame
    void Update () {
        int collisionState = physics.getPlayerCollisionState();

        if(collisionState == CollisionStates.GROUND || collisionState == CollisionStates.WALL) {
            airJumpsLeft = MAX_AIR_JUMPS;
        }

        if(!isInputLocked()) {
            float xInput = Input.GetAxisRaw("Horizontal");

            if(collisionState == CollisionStates.GROUND) {
                physics.setVelocityX(HORIZONTAL_GROUND_SPEED * xInput);
            } else {
                physics.setVelocityX(HORIZONTAL_AIR_SPEED * xInput);
            }

            bool jumpTrigger = Input.GetButtonDown("Jump");
            if(jumpTrigger) {
                if(collisionState == CollisionStates.WALL && wallJumpingEnabled) {
                    if(physics.cols.isCollisionAtDirection(physics.cols.left, Vector2.left)) {
                        physics.setVelocity(JUMP_VELOCITY, JUMP_VELOCITY);
                    } else {
                        physics.setVelocity(-JUMP_VELOCITY, JUMP_VELOCITY);
                    }
                    inputLockTimer = WALL_JUMP_INPUT_LOCK;
                } else if(collisionState == CollisionStates.GROUND) {
                    physics.setVelocityY(JUMP_VELOCITY);
                } else if(collisionState == CollisionStates.AIR && airJumpsEnabled && airJumpsLeft > 0) {
                        airJumpsLeft--;
                        physics.setVelocityY(AIR_JUMP_VELOCITY);
                }
            }
        }
    }

    /// <summary>
    ///     Determine if the user's input is time-locked.
    /// </summary>
    /// <returns>True if the user's input is locked, false otherwise.</returns>
    public bool isInputLocked() {
        return inputLockTimer != 0f;
    }
}
